using System.Net;
using System.Text.Json;
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

namespace ButceYonet.Application.Application.Features.RecurringTransactions.CreateRecurringTransaction;

public class CreateRecurringTransactionCommandHandler : BaseHandler<CreateRecurringTransactionCommand, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;
    
    public CreateRecurringTransactionCommandHandler(
        ICache cache, 
        IUser user,
        IMapper mapper, 
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _notebookLabelRepository = notebookLabelRepository;
        _recurringTransactionRepository = recurringTransactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateRecurringTransactionCommand request, CancellationToken cancellationToken)
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

        var transactions = new List<Transaction>();
        
        var notebookLabels = await
            _notebookLabelRepository
                .GetAll()
                .Where(nl => nl.NotebookId == request.NotebookId)
                .ToListAsync();
        
        var transaction = new Transaction
        {
            NotebookId = request.NotebookId,
            ExternalId = "",
            Name = request.Transaction.Name,
            Description = request.Transaction.Description,
            Amount = request.Transaction.Amount,
            CurrencyId = request.Transaction.CurrencyId,
            TransactionType = request.Transaction.TransactionType,
            TransactionDate = request.Transaction.TransactionDate
        };

        transaction.TransactionLabels = notebookLabels
            .Where(nl => request.Transaction.Labels.Contains(nl.Id))
            .Select(nl => new TransactionLabel
            {
                NotebookLabelId = nl.NotebookId
            }).ToList();

        transaction.IsMatched = transaction.TransactionLabels.Any();
        transactions.Add(transaction);

        var recurringTransaction = new RecurringTransaction
        {
            NotebookId = request.NotebookId,
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Frequency = request.Frequency,
            Interval = request.Interval,
            NextOccurrence = request.StartDate,
            StateData = JsonSerializer.Serialize(transactions)
        };

        var recurringTransactionAddedDomainEvent = new RecurringTransactionAddedDomainEvent(recurringTransaction);
        recurringTransaction.AddEvent(recurringTransactionAddedDomainEvent);

        await _recurringTransactionRepository.AddAsync(recurringTransaction);
        await _recurringTransactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}