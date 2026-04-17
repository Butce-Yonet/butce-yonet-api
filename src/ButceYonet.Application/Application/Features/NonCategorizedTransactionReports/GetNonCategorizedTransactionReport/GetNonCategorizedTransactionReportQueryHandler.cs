using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
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

namespace ButceYonet.Application.Application.Features.NonCategorizedTransactionReports.GetNonCategorizedTransactionReport;

public class GetNonCategorizedTransactionReportQueryHandler : BaseHandler<GetNonCategorizedTransactionReportQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    
    private readonly IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>
        _nonCategorizedTransactionReportRepository;
    
    public GetNonCategorizedTransactionReportQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> nonCategorizedTransactionReportRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _nonCategorizedTransactionReportRepository = nonCategorizedTransactionReportRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetNonCategorizedTransactionReportQuery request, CancellationToken cancellationToken)
    {
        var report = await this._cache.GetOrSetAsync<List<NonCategorizedTransactionReportDto>>(request.ToString(), async () =>
        {
            var isNotebookUser = await
                _notebookUserRepository
                    .Get()
                    .Where(nu =>
                        nu.NotebookId == request.NotebookId &&
                        nu.UserId == _user.Id)
                    .AnyAsync();

            if (!isNotebookUser)
                return new List<NonCategorizedTransactionReportDto>();
            
            var reportItems = await _nonCategorizedTransactionReportRepository
                .GetAll()
                .Where(nctr =>
                    nctr.NotebookId == request.NotebookId &&
                    nctr.TransactionType == request.TransactionTypes)
                .WhereIf(request.CurrencyId.HasValue, nctr => nctr.CurrencyId == request.CurrencyId)
                .WhereIf(request.StartDate.HasValue, nctr => nctr.Term >= request.StartDate)
                .WhereIf(request.EndDate.HasValue, nctr => nctr.Term <= request.EndDate)
                .Include(nctr => nctr.Notebook)
                .Include(nctr => nctr.Currency)
                .ToListAsync();

            var grouped = reportItems
                .GroupBy(nctr => new { nctr.CurrencyId, nctr.Term.Year, nctr.Term.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.CurrencyId,
                    Amount = g.Sum(x => x.Amount),
                    First = g.First()
                })
                .ToList();

            var responseModel = grouped
                .GroupBy(g => g.CurrencyId)
                .SelectMany(g =>
                {
                    decimal cumulative = 0;
                    return g.OrderBy(x => x.Year).ThenBy(x => x.Month)
                        .Select(x =>
                        {
                            cumulative += x.Amount;
                            var s = x.First;
                            return new NonCategorizedTransactionReportDto
                            {
                                NotebookDto = new NotebookDto { Id = s.Notebook.Id, Name = s.Notebook.Name, IsDefault = s.Notebook.IsDefault },
                                TransactionTypes = s.TransactionType,
                                Currency = new CurrencyDto { Id = s.Currency.Id, Code = s.Currency.Code, Name = s.Currency.Name, Symbol = s.Currency.Symbol, IsSymbolRight = s.Currency.IsSymbolRight, Rank = s.Currency.Rank },
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