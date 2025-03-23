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

            var responseModel =
                _mapper.Map<List<CategorizedTransactionReport>, List<CategorizedTransactionReportDto>>(reportItems);
            return BaseResponse.Response(responseModel, HttpStatusCode.OK);
        }, TimeSpan.FromMinutes(15));
    }
}