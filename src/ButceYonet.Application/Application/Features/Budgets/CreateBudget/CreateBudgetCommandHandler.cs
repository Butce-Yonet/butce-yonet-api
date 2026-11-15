using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Budgets.CreateBudget;

public class CreateBudgetCommandHandler : BaseHandler<CreateBudgetCommand, BaseResponse>
{
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;

    public CreateBudgetCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Budget, ButceYonetDbContext> budgetRepository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<Currency, ButceYonetDbContext> currencyRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _budgetRepository = budgetRepository;
        _userLabelRepository = userLabelRepository;
        _currencyRepository = currencyRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var currencyExists = await _currencyRepository
            .GetAll()
            .AnyAsync(c => c.Id == request.CurrencyId, cancellationToken);

        if (!currencyExists)
            throw new NotFoundException(typeof(Currency));

        if (request.UserLabelId.HasValue)
        {
            var labelExists = await _userLabelRepository
                .GetAll()
                .AnyAsync(ul =>
                    ul.Id == request.UserLabelId.Value &&
                    (ul.UserId == null || ul.UserId == _user.Id), cancellationToken);

            if (!labelExists)
                throw new NotFoundException(typeof(UserLabel));
        }

        // Aynı etiket (ya da genel) + para birimi için tek bütçe. MySQL'de filtrelenmiş unique index
        // olmadığı için burada kontrol ediliyor (bkz. BudgetEntityTypeConfiguration).
        var alreadyExists = await _budgetRepository
            .GetAll()
            .AnyAsync(b =>
                b.UserId == _user.Id &&
                b.CurrencyId == request.CurrencyId &&
                b.UserLabelId == request.UserLabelId, cancellationToken);

        if (alreadyExists)
            throw new AlreadyExistsException(typeof(Budget));

        var budget = new Budget
        {
            UserId = _user.Id,
            UserLabelId = request.UserLabelId,
            Amount = request.Amount,
            CurrencyId = request.CurrencyId,
            PeriodStartDay = request.PeriodStartDay
        };

        await _budgetRepository.AddAsync(budget);
        await _budgetRepository.SaveChangesAsync();

        return BaseResponse.Response(new { budget.Id }, HttpStatusCode.OK);
    }
}
