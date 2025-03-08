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

namespace ButceYonet.Application.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryHandler : BaseHandler<GetTransactionsQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    
    public GetTransactionsQueryHandler(
        ICache cache, 
        IUser user, 
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<Transaction, ButceYonetDbContext> transactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _transactionRepository = transactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var isNotebookUser = await
            _notebookUserRepository
                .Get()
                .Where(nu =>
                    nu.NotebookId == request.NotebookId &&
                    nu.UserId == _user.Id)
                .AnyAsync();
        
        if (!isNotebookUser)
            throw new BusinessRuleException("User is not in notebook"); //TODO:

        var transactions = await
            _transactionRepository
                .GetAll()
                .Where(t =>
                    t.NotebookId == request.NotebookId)
                .WhereIf(request.StartTime.HasValue, t => t.TransactionDate >= request.StartTime)
                .WhereIf(request.EndTime.HasValue, t => t.TransactionDate <= request.EndTime)
                .Include(t => t.Notebook)
                .Include(t => t.Currency)
                .Include(t => t.TransactionLabels)
                .ThenInclude(tl => tl.NotebookLabel)
                .PaginateAsync(request);

        var paginateItems = _mapper.Map<List<TransactionDto>>(transactions.Items);
        var paginatedResponse = new PaginatedModel<TransactionDto>(transactions.PageNumber, transactions.PageSize,
            transactions.TotalPages, transactions.TotalRecords, paginateItems);

        return BaseResponse.Response(paginatedResponse, HttpStatusCode.OK);
    }
}