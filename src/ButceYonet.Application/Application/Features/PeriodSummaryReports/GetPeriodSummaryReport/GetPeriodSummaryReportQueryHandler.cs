using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.PeriodSummaryReports.GetPeriodSummaryReport;

public class GetPeriodSummaryReportQueryHandler : BaseHandler<GetPeriodSummaryReportQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedTransactionReportRepository;
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;
    private readonly IMapper _mapper;

    public GetPeriodSummaryReportQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> nonCategorizedTransactionReportRepository,
        IRepository<Currency, ButceYonetDbContext> currencyRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _nonCategorizedTransactionReportRepository = nonCategorizedTransactionReportRepository;
        _currencyRepository = currencyRepository;
        _mapper = mapper;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetPeriodSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var isNotebookUser = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.UserId == _user.Id)
            .AnyAsync(cancellationToken);

        if (!isNotebookUser)
            throw new BusinessRuleException("User is not in notebook");

        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date;

        var baseQuery = _nonCategorizedTransactionReportRepository
            .GetAll()
            .Where(nctr => nctr.NotebookId == request.NotebookId)
            .WhereIf(request.CurrencyId.HasValue, nctr => nctr.CurrencyId == request.CurrencyId.Value);

        var totalIncome = await baseQuery
            .Where(nctr => nctr.TransactionType == TransactionTypes.Income && nctr.Term >= startDate && nctr.Term <= endDate)
            .SumAsync(nctr => nctr.Amount, cancellationToken);

        var totalExpense = await baseQuery
            .Where(nctr => nctr.TransactionType == TransactionTypes.Expense && nctr.Term >= startDate && nctr.Term <= endDate)
            .SumAsync(nctr => nctr.Amount, cancellationToken);

        var netBalance = totalIncome - totalExpense;

        var periodLengthDays = (endDate - startDate).Days + 1;
        var prevEndDate = startDate.AddDays(-1);
        var prevStartDate = prevEndDate.AddDays(-periodLengthDays + 1);

        decimal? previousTotalIncome = null;
        decimal? previousTotalExpense = null;
        decimal? previousNetBalance = null;
        decimal? incomeChangePercent = null;
        decimal? expenseChangePercent = null;
        decimal? netBalanceChangePercent = null;

        if (prevStartDate <= prevEndDate)
        {
            previousTotalIncome = await baseQuery
                .Where(nctr => nctr.TransactionType == TransactionTypes.Income && nctr.Term >= prevStartDate && nctr.Term <= prevEndDate)
                .SumAsync(nctr => nctr.Amount, cancellationToken);

            previousTotalExpense = await baseQuery
                .Where(nctr => nctr.TransactionType == TransactionTypes.Expense && nctr.Term >= prevStartDate && nctr.Term <= prevEndDate)
                .SumAsync(nctr => nctr.Amount, cancellationToken);

            previousNetBalance = previousTotalIncome.Value - previousTotalExpense.Value;

            incomeChangePercent = CalculateChangePercent(totalIncome, previousTotalIncome.Value);
            expenseChangePercent = CalculateChangePercent(totalExpense, previousTotalExpense.Value);
            netBalanceChangePercent = CalculateChangePercent(netBalance, previousNetBalance.Value);
        }

        CurrencyDto currencyDto = null;
        if (request.CurrencyId.HasValue)
        {
            var currency = await _currencyRepository.Get()
                .Where(c => c.Id == request.CurrencyId.Value)
                .FirstOrDefaultAsync(cancellationToken);
            if (currency != null)
                currencyDto = _mapper.Map<CurrencyDto>(currency);
        }

        var dto = new PeriodSummaryReportDto
        {
            NotebookId = request.NotebookId,
            StartDate = startDate,
            EndDate = endDate,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetBalance = netBalance,
            PreviousPeriodStartDate = prevStartDate <= prevEndDate ? prevStartDate : (DateTime?)null,
            PreviousPeriodEndDate = prevStartDate <= prevEndDate ? prevEndDate : (DateTime?)null,
            PreviousTotalIncome = previousTotalIncome,
            PreviousTotalExpense = previousTotalExpense,
            PreviousNetBalance = previousNetBalance,
            IncomeChangePercent = incomeChangePercent,
            ExpenseChangePercent = expenseChangePercent,
            NetBalanceChangePercent = netBalanceChangePercent,
            Currency = currencyDto
        };

        return BaseResponse.Response(dto, HttpStatusCode.OK);
    }

    private static decimal? CalculateChangePercent(decimal current, decimal previous)
    {
        if (previous == 0)
            return current == 0 ? 0 : null;
        return Math.Round(((current - previous) / previous) * 100, 2);
    }
}
