using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQueryHandler : BaseHandler<GetTransactionQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    
    public GetTransactionQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetTransactionQuery request, CancellationToken cancellationToken)
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

        var transaction = await
            _transactionRepository
                .Get()
                .Where(t =>
                    t.NotebookId == request.NotebookId &&
                    t.Id == request.TransactionId)
                .Include(t => t.Notebook)
                .Include(t => t.Currency)
                .Include(t => t.TransactionLabels)
                .ThenInclude(tl => tl.NotebookLabel)
                .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException(typeof(Transaction));

        var responseDto = _mapper.Map<TransactionDto>(transaction);
        
        return BaseResponse.Response(responseDto, HttpStatusCode.OK);
    }
}