using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Budgets.GetBudgets;

// Bilerek cache'lenmiyor: kullanıcı işlem girdikten hemen sonra bütçe çubuğunun dolduğunu görmeli.
public class GetBudgetsQueryHandler : BaseHandler<GetBudgetsQuery, BaseResponse>
{
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;
    private readonly IBudgetSpendingCalculator _spendingCalculator;

    public GetBudgetsQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Budget, ButceYonetDbContext> budgetRepository,
        IBudgetSpendingCalculator spendingCalculator)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _budgetRepository = budgetRepository;
        _spendingCalculator = spendingCalculator;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _budgetRepository
            .GetAll()
            .Where(b => b.UserId == _user.Id)
            .WhereIf(request.CurrencyId.HasValue, b => b.CurrencyId == request.CurrencyId.Value)
            .Include(b => b.Currency)
            .Include(b => b.UserLabel)
            .ToListAsync(cancellationToken);

        var dtos = await BudgetStatusBuilder.BuildAsync(budgets, _spendingCalculator, _mapper, _user.Id, DateTime.Now, cancellationToken);

        // Genel bütçe en üstte, ardından en çok dolan bütçeler.
        var ordered = dtos
            .OrderByDescending(d => d.IsGeneral)
            .ThenByDescending(d => d.Percentage)
            .ToList();

        return BaseResponse.Response(ordered, HttpStatusCode.OK);
    }
}
