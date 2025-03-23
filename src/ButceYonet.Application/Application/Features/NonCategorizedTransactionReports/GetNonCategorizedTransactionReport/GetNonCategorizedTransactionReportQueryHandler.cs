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
        return await this._cache.GetOrSetAsync(request.ToString(), async () =>
        {
            var isNotebookUser = await
                _notebookUserRepository
                    .Get()
                    .Where(nu =>
                        nu.NotebookId == request.NotebookId &&
                        nu.UserId == _user.Id)
                    .AnyAsync();

            if (!isNotebookUser)
                return BaseResponse.Response(Array.Empty<CategorizedTransactionReportDto>(), HttpStatusCode.OK);
            
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

            var responseModel = _mapper.Map<List<NonCategorizedTransactionReportDto>>(reportItems);
            return BaseResponse.Response(responseModel, HttpStatusCode.OK);
        }, TimeSpan.FromMinutes(15));
    }
}