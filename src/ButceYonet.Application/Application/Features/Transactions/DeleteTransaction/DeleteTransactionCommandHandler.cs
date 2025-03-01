using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandHandler : BaseHandler<DeleteTransactionCommand, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    
    public DeleteTransactionCommandHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var isNotebookUser = await
            _notebookUserRepository
                .Get()
                .Where(nu =>
                    nu.NotebookId == request.NotebookId &&
                    nu.UserId == _user.Id)
                .AnyAsync();

        if (!isNotebookUser)
            throw new BusinessRuleException(""); //TODO:

        var transaction = await
            _transactionRepository
                .Get()
                .Where(t =>
                    t.NotebookId == request.NotebookId &&
                    t.Id == request.TransactionId)
                .FirstOrDefaultAsync();
        
        if (transaction is null)
            throw new NotFoundException(typeof(Transaction));

        var transactionDeletedDomainEvent = new TransactionDeletedDomainEvent(transaction);
        transaction.AddEvent(transactionDeletedDomainEvent);
        
        _transactionRepository.Remove(transaction);
        await _transactionRepository.SaveChangesAsync();
            
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}