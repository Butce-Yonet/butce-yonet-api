using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Budgets.GetBudget;

public class GetBudgetQueryHandler : BaseHandler<GetBudgetQuery, BaseResponse>
{
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;
    private readonly IBudgetSpendingCalculator _spendingCalculator;

    public GetBudgetQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetBudgetQuery request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository
            .Get()
            .Where(b => b.Id == request.BudgetId && b.UserId == _user.Id)
            .Include(b => b.Currency)
            .Include(b => b.UserLabel)
            .FirstOrDefaultAsync(cancellationToken);

        if (budget is null)
            throw new NotFoundException(typeof(Budget));

        var asOf = DateTime.Now;
        var periods = BudgetPeriodCalculator.GetPeriods(budget.PeriodStartDay, asOf, request.HistoryCount);
        var snapshot = await _spendingCalculator.LoadAsync(_user.Id, periods[0].Start, periods[^1].End, cancellationToken);

        var dto = _mapper.Map<BudgetDto>(budget);
        var (currentStart, currentEnd) = periods[^1];
        BudgetProgressCalculator.Apply(dto, snapshot.GetSpent(budget, currentStart, currentEnd), currentStart, currentEnd, asOf);

        dto.History = periods
            .Select(p => new BudgetPeriodDto
            {
                PeriodStart = p.Start,
                PeriodEnd = p.End,
                SpentAmount = snapshot.GetSpent(budget, p.Start, p.End)
            })
            .ToList();

        return BaseResponse.Response(dto, HttpStatusCode.OK);
    }
}
