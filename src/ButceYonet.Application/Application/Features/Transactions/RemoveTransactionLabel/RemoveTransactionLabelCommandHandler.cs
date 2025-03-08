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
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.RemoveTransactionLabel;

public class RemoveTransactionLabelCommandHandler : BaseHandler<RemoveTransactionLabelCommand, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    private readonly IRepository<TransactionLabel, ButceYonetDbContext> _transactionLabelRepository;
    
    public RemoveTransactionLabelCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize, 
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<Transaction, ButceYonetDbContext> transactionRepository,
        IRepository<TransactionLabel, ButceYonetDbContext> transactionLabelRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _transactionRepository = transactionRepository;
        _transactionLabelRepository = transactionLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(RemoveTransactionLabelCommand request, CancellationToken cancellationToken)
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
                .Include(t => t.TransactionLabels)
                .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException(typeof(Transaction));
        
        var transactionLabel = transaction.TransactionLabels.Where(tl => tl.NotebookLabelId == request.LabelId).FirstOrDefault();

        if (transactionLabel != null && transaction.TransactionLabels.Count == 1)
            throw new BusinessRuleException("There must be at least one label for the income-expense record"); //TODO:

        var transactionLabelRemoved =
            new TransactionLabelRemovedDomainEvent(transaction.Id, transactionLabel.NotebookLabelId);
        transactionLabel.AddEvent(transactionLabelRemoved);
        
        _transactionLabelRepository.Remove(transactionLabel);
        await _transactionLabelRepository.SaveChangesAsync();
        
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}