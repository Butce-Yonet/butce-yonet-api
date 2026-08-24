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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryHandler : BaseHandler<GetTransactionsQuery, BaseResponse>
{
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;
    private IHttpContextAccessor _httpContextAccessor;

    public GetTransactionsQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository,
        IHttpContextAccessor httpContextAccessor)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _transactionRepository = transactionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = new PaginationFilter(
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageNumber"].ToString()),
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageSize"].ToString()));

        var transactions = await
            _transactionRepository
                .GetAll()
                .Where(t => t.NotebookV2.UserId == _user.Id)
                .WhereIf(request.NotebookId.HasValue, t => t.NotebookV2Id == request.NotebookId)
                .WhereIf(request.StartTime.HasValue, t => t.TransactionDate >= request.StartTime)
                .WhereIf(request.EndTime.HasValue, t => t.TransactionDate <= request.EndTime)
                .WhereIf(!string.IsNullOrEmpty(request.Name), t => t.Name.Contains(request.Name))
                .WhereIf(!string.IsNullOrEmpty(request.Description), t => t.Description.Contains(request.Description))
                .WhereIf(request.Amount > 0, t => t.Amount == request.Amount)
                .WhereIf(request.TransactionType != default, t => t.TransactionType == request.TransactionType)
                .WhereIf(
                    request.LabelIds != null && request.LabelIds.Any(),
                    t => t.TransactionLabelsV2.Any(tl => !tl.IsDeleted && request.LabelIds.Contains(tl.UserLabelId)))
                .Include(t => t.NotebookV2)
                .Include(t => t.Currency)
                .Include(t => t.TransactionLabelsV2.Where(tl => !tl.IsDeleted))
                .ThenInclude(tl => tl.UserLabel)
                .OrderByDescending(t => t.TransactionDate)
                .PaginateAsync(paginationRequest);

        var paginateItems = _mapper.Map<List<TransactionDto>>(transactions.Items);
        var paginatedResponse = new PaginatedModel<TransactionDto>(transactions.PageNumber, transactions.PageSize,
            transactions.TotalPages, transactions.TotalRecords, paginateItems);

        return BaseResponse.Response(paginatedResponse, HttpStatusCode.OK);
    }
}
