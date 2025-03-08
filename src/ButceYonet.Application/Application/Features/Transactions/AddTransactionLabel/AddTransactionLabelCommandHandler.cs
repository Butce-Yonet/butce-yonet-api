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

namespace ButceYonet.Application.Application.Features.Transactions.AddTransactionLabel;

public class AddTransactionLabelCommandHandler : BaseHandler<AddTransactionLabelCommand, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    private readonly IRepository<TransactionLabel, ButceYonetDbContext> _transactionLabelRepository;
    
    public AddTransactionLabelCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper, 
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
    }

    public override async Task<BaseResponse> ExecuteRequest(AddTransactionLabelCommand request, CancellationToken cancellationToken)
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
                .Where(t => t.Id == request.TransactionId)
                .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException(typeof(Transaction));

        var transactionLabelAlreadyExists = await
            _transactionLabelRepository
                .Get()
                .Where(tl =>
                    tl.TransactionId == request.TransactionId &&
                    tl.NotebookLabelId == request.LabelId)
                .AnyAsync();

        if (transactionLabelAlreadyExists)
            throw new AlreadyExistsException(typeof(TransactionLabel));

        var notebookLabels = await
            _notebookLabelRepository
                .GetAll()
                .Where(nl => nl.NotebookId == request.NotebookId)
                .ToListAsync();

        var notebookLabel = notebookLabels.Where(nl => nl.Id == request.LabelId).FirstOrDefault();
         
        if (notebookLabel is null)
            throw new NotFoundException(typeof(NotebookLabel));

        var transactionLabel = new TransactionLabel
        {
            TransactionId = request.TransactionId,
            NotebookLabelId = request.LabelId,
        };

        var transactionLabelAddedDomainEvent = new TransactionLabelAddedDomainEvent(request.TransactionId, request.LabelId);
        transactionLabel.AddEvent(transactionLabelAddedDomainEvent);

        await _transactionLabelRepository.AddAsync(transactionLabel);
        await _transactionLabelRepository.SaveChangesAsync();
        
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}