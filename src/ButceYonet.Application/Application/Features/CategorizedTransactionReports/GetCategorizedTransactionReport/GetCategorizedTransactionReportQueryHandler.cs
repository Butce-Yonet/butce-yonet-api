using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.CategorizedTransactionReports.GetCategorizedTransactionReport;

public class GetCategorizedTransactionReportQueryHandler : BaseHandler<GetCategorizedTransactionReportQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<CategorizedTransactionReport, ButceYonetDbContext>
        _categorizedTransactionReportRepository;
    
    public GetCategorizedTransactionReportQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper, 
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<CategorizedTransactionReport, ButceYonetDbContext> categorizedTransactionReportRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _categorizedTransactionReportRepository = categorizedTransactionReportRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetCategorizedTransactionReportQuery request, CancellationToken cancellationToken)
    {
        var report =  await this._cache.GetOrSetAsync<List<CategorizedTransactionReportDto>>(request.ToString(), async () =>
        {
            var isNotebookUser = await
                _notebookUserRepository
                    .Get()
                    .Where(nu =>
                        nu.NotebookId == request.NotebookId &&
                        nu.UserId == _user.Id)
                    .AnyAsync();

            if (!isNotebookUser)
                return new List<CategorizedTransactionReportDto>();
            
            var reportItems = await
                _categorizedTransactionReportRepository
                    .GetAll()
                    .Where(ctr =>
                        ctr.NotebookId == request.NotebookId &&
                        ctr.TransactionType == request.TransactionTypes)
                    .WhereIf(request.NotebookLabelId.HasValue, ctr => ctr.NotebookLabelId == request.NotebookLabelId)
                    .WhereIf(request.CurrencyId.HasValue, ctr => ctr.CurrencyId == request.CurrencyId)
                    .WhereIf(request.StartDate.HasValue, ctr => ctr.Term >= request.StartDate)
                    .WhereIf(request.EndDate.HasValue, ctr => ctr.Term <= request.EndDate)
                    .Include(ctr => ctr.Notebook)
                    .Include(ctr => ctr.NotebookLabel)
                    .Include(ctr => ctr.Currency)
                    .ToListAsync();

            var grouped = reportItems
                .GroupBy(ctr => new { ctr.NotebookLabelId, ctr.CurrencyId, ctr.Term.Year, ctr.Term.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.NotebookLabelId,
                    g.Key.CurrencyId,
                    Amount = g.Sum(x => x.Amount),
                    First = g.First()
                })
                .ToList();

            var responseModel = grouped
                .GroupBy(g => new { g.NotebookLabelId, g.CurrencyId })
                .SelectMany(g =>
                {
                    decimal cumulative = 0;
                    return g.OrderBy(x => x.Year).ThenBy(x => x.Month)
                        .Select(x =>
                        {
                            cumulative += x.Amount;
                            var s = x.First;
                            return new CategorizedTransactionReportDto
                            {
                                Notebook = new NotebookDto { Id = s.Notebook.Id, Name = s.Notebook.Name, IsDefault = s.Notebook.IsDefault },
                                NotebookLabel = new NotebookLabelDto { Id = s.NotebookLabel.Id, Name = s.NotebookLabel.Name, ColorCode = s.NotebookLabel.ColorCode },
                                TransactionType = s.TransactionType,
                                Currency = new CurrencyDto { Id = s.Currency.Id, Code = s.Currency.Code, Name = s.Currency.Name, Symbol = s.Currency.Symbol, IsSymbolRight = s.Currency.IsSymbolRight },
                                Amount = cumulative,
                                Term = new DateTime(x.Year, x.Month, 1)
                            };
                        });
                })
                .OrderBy(x => x.Term)
                .ToList();

            return responseModel;
        }, TimeSpan.FromMinutes(15));
        
        return BaseResponse.Response(report, HttpStatusCode.OK);
    }
}