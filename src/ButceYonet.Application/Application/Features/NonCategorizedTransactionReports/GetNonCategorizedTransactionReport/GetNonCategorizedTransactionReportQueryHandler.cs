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
    private readonly IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>
        _nonCategorizedTransactionReportRepository;

    public GetNonCategorizedTransactionReportQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> nonCategorizedTransactionReportRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _nonCategorizedTransactionReportRepository = nonCategorizedTransactionReportRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetNonCategorizedTransactionReportQuery request, CancellationToken cancellationToken)
    {
        var report = await this._cache.GetOrSetAsync<List<NonCategorizedTransactionReportDto>>($"{request}:{_user.Id}", async () =>
        {
            var reportItems = await _nonCategorizedTransactionReportRepository
                .GetAll()
                .Where(nctr =>
                    nctr.NotebookV2.UserId == _user.Id &&
                    nctr.TransactionType == request.TransactionTypes)
                .WhereIf(request.CurrencyId.HasValue, nctr => nctr.CurrencyId == request.CurrencyId)
                .WhereIf(request.StartDate.HasValue, nctr => nctr.Term >= request.StartDate)
                .WhereIf(request.EndDate.HasValue, nctr => nctr.Term <= request.EndDate)
                .Include(nctr => nctr.NotebookV2)
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
                                NotebookDto = new NotebookDto { Id = s.NotebookV2.Id, Name = s.NotebookV2.Name, TermStart = s.NotebookV2.TermStart, TermEnd = s.NotebookV2.TermEnd },
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
