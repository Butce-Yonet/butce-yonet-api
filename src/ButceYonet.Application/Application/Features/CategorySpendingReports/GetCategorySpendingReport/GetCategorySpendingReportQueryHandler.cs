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

namespace ButceYonet.Application.Application.Features.CategorySpendingReports.GetCategorySpendingReport;

public class GetCategorySpendingReportQueryHandler : BaseHandler<GetCategorySpendingReportQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<CategorizedTransactionReport, ButceYonetDbContext> _categorizedTransactionReportRepository;
    private readonly IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedTransactionReportRepository;

    public GetCategorySpendingReportQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<CategorizedTransactionReport, ButceYonetDbContext> categorizedTransactionReportRepository,
        IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> nonCategorizedTransactionReportRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _categorizedTransactionReportRepository = categorizedTransactionReportRepository;
        _nonCategorizedTransactionReportRepository = nonCategorizedTransactionReportRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetCategorySpendingReportQuery request, CancellationToken cancellationToken)
    {
        var isNotebookUser = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.UserId == _user.Id)
            .AnyAsync(cancellationToken);

        if (!isNotebookUser)
            throw new BusinessRuleException("User is not in notebook");

        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date;

        // Kategorize edilmiş giderler (sadece Expense)
        var categorizedBaseQuery = _categorizedTransactionReportRepository
            .GetAll()
            .Where(ctr => ctr.NotebookId == request.NotebookId && ctr.TransactionType == TransactionTypes.Expense)
            .WhereIf(request.CurrencyId.HasValue, ctr => ctr.CurrencyId == request.CurrencyId.Value);

        var currentCategorized = await categorizedBaseQuery
            .Where(ctr => ctr.Term >= startDate && ctr.Term <= endDate)
            .Include(ctr => ctr.NotebookLabel)
            .ToListAsync(cancellationToken);

        // Genel toplam gider (kategori bağımsız) NonCategorizedTransactionReport üzerinden
        var nonCategorizedBaseQuery = _nonCategorizedTransactionReportRepository
            .GetAll()
            .Where(nctr => nctr.NotebookId == request.NotebookId && nctr.TransactionType == TransactionTypes.Expense)
            .WhereIf(request.CurrencyId.HasValue, nctr => nctr.CurrencyId == request.CurrencyId.Value);

        var generalTotalAmount = await nonCategorizedBaseQuery
            .Where(nctr => nctr.Term >= startDate && nctr.Term <= endDate)
            .SumAsync(nctr => nctr.Amount, cancellationToken);

        // Önceki dönem tarihleri
        var periodLengthDays = (endDate - startDate).Days + 1;
        var prevEndDate = startDate.AddDays(-1);
        var prevStartDate = prevEndDate.AddDays(-periodLengthDays + 1);

        var previousCategorized = prevStartDate <= prevEndDate
            ? await categorizedBaseQuery
                .Where(ctr => ctr.Term >= prevStartDate && ctr.Term <= prevEndDate)
                .ToListAsync(cancellationToken)
            : new List<CategorizedTransactionReport>();

        var previousByLabel = previousCategorized
            .GroupBy(i => i.NotebookLabelId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        var items = currentCategorized
            .GroupBy(i => i.NotebookLabelId)
            .Select(g =>
            {
                var amount = g.Sum(x => x.Amount);
                previousByLabel.TryGetValue(g.Key, out var prevAmount);

                var percentage = generalTotalAmount == 0
                    ? 0
                    : Math.Round((amount / generalTotalAmount) * 100, 2);

                return new CategorySpendingReportItemDto
                {
                    CategoryName = g.First().NotebookLabel.Name,
                    Amount = amount,
                    Percentage = percentage,
                    PreviousAmount = prevAmount
                };
            })
            .OrderByDescending(i => i.Amount)
            .ToList();

        var dto = new CategorySpendingReportDto
        {
            NotebookId = request.NotebookId,
            StartDate = startDate,
            EndDate = endDate,
            GeneralTotalAmount = generalTotalAmount,
            Items = items
        };

        return BaseResponse.Response(dto, HttpStatusCode.OK);
    }
}

