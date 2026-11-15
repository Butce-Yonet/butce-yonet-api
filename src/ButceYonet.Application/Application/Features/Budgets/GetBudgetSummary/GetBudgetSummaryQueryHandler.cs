using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Budgets.GetBudgetSummary;

public class GetBudgetSummaryQueryHandler : BaseHandler<GetBudgetSummaryQuery, BaseResponse>
{
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;
    private readonly IBudgetSpendingCalculator _spendingCalculator;

    public GetBudgetSummaryQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _budgetRepository
            .GetAll()
            .Where(b => b.UserId == _user.Id)
            .Include(b => b.Currency)
            .Include(b => b.UserLabel)
            .ToListAsync(cancellationToken);

        var dtos = await BudgetStatusBuilder.BuildAsync(budgets, _spendingCalculator, _mapper, _user.Id, DateTime.Now, cancellationToken);

        var summaries = dtos
            .GroupBy(d => d.Currency.Id)
            .Select(g =>
            {
                var general = g.FirstOrDefault(d => d.IsGeneral);
                var labelBudgets = g.Where(d => !d.IsGeneral).ToList();

                var totalAmount = general?.Amount ?? labelBudgets.Sum(d => d.Amount);
                var totalSpent = general?.SpentAmount ?? labelBudgets.Sum(d => d.SpentAmount);

                return new BudgetSummaryDto
                {
                    Currency = g.First().Currency,
                    TotalAmount = totalAmount,
                    TotalSpent = totalSpent,
                    RemainingAmount = Math.Max(0, totalAmount - totalSpent),
                    BudgetCount = g.Count(),
                    OnTrackCount = g.Count(d => d.Status == BudgetStatus.OnTrack),
                    WarningCount = g.Count(d => d.Status == BudgetStatus.Warning),
                    ExceededCount = g.Count(d => d.Status == BudgetStatus.Exceeded)
                };
            })
            .OrderBy(s => s.Currency.Rank)
            .ToList();

        return BaseResponse.Response(summaries, HttpStatusCode.OK);
    }
}
