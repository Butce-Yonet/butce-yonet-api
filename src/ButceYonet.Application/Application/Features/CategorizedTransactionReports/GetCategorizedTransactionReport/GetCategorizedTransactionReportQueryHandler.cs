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
    private readonly IRepository<CategorizedTransactionReportV2, ButceYonetDbContext> _categorizedTransactionReportRepository;

    public GetCategorizedTransactionReportQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<CategorizedTransactionReportV2, ButceYonetDbContext> categorizedTransactionReportRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _categorizedTransactionReportRepository = categorizedTransactionReportRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetCategorizedTransactionReportQuery request, CancellationToken cancellationToken)
    {
        var report = await this._cache.GetOrSetAsync<List<CategorizedTransactionReportDto>>(request.ToString(), async () =>
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
                    .WhereIf(request.NotebookLabelId.HasValue, ctr => ctr.UserLabelId == request.NotebookLabelId)
                    .WhereIf(request.CurrencyId.HasValue, ctr => ctr.CurrencyId == request.CurrencyId)
                    .WhereIf(request.StartDate.HasValue, ctr => ctr.Term >= request.StartDate)
                    .WhereIf(request.EndDate.HasValue, ctr => ctr.Term <= request.EndDate)
                    .Include(ctr => ctr.Notebook)
                    .Include(ctr => ctr.UserLabel)
                    .Include(ctr => ctr.Currency)
                    .ToListAsync();

            var responseModel = reportItems
                .GroupBy(ctr => new { ctr.UserLabelId, ctr.CurrencyId, ctr.Term.Year, ctr.Term.Month })
                .Select(g =>
                {
                    var s = g.First();
                    return new CategorizedTransactionReportDto
                    {
                        Notebook = new NotebookDto { Id = s.Notebook.Id, Name = s.Notebook.Name, IsDefault = s.Notebook.IsDefault },
                        UserLabel = new UserLabelDto { Id = s.UserLabel.Id, Name = s.UserLabel.Name, ColorCode = s.UserLabel.ColorCode },
                        TransactionType = s.TransactionType,
                        Currency = new CurrencyDto { Id = s.Currency.Id, Code = s.Currency.Code, Name = s.Currency.Name, Symbol = s.Currency.Symbol, IsSymbolRight = s.Currency.IsSymbolRight, Rank = s.Currency.Rank },
                        Amount = g.Sum(x => x.Amount),
                        Term = new DateTime(g.Key.Year, g.Key.Month, 1)
                    };
                })
                .OrderBy(x => x.Term)
                .ToList();

            return responseModel;
        }, TimeSpan.FromSeconds(5));

        return BaseResponse.Response(report, HttpStatusCode.OK);
    }
}