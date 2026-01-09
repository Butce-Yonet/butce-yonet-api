using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandHandler : BaseHandler<CreateTransactionCommand, BaseResponse>
{
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    
    public CreateTransactionCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper,
        ILocalize localize, 
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<Transaction, ButceYonetDbContext> transactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookLabelRepository = notebookLabelRepository;
        _notebookUserRepository = notebookUserRepository;
        _transactionRepository = transactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateTransactionCommand request, CancellationToken cancellationToken)
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

        var notebookTransactionCountValidateParameters = new Dictionary<string, string>
        {
            { "NotebookId", request.NotebookId.ToString()}
        };

        await _userPlanValidator.Validate(PlanFeatures.NotebookTransactionCount, notebookTransactionCountValidateParameters);

        var notebookLabels = await
            _notebookLabelRepository
                .GetAll()
                .Where(nl => nl.NotebookId == request.NotebookId)
                .ToListAsync();

        foreach (var requestItem in request.Transactions)
        {
            var transaction = new Transaction
            {
                NotebookId = request.NotebookId,
                ExternalId = Guid.NewGuid().ToString(),
                Name = requestItem.Name,
                Description = requestItem.Description,
                Amount = requestItem.Amount,
                CurrencyId = requestItem.CurrencyId,
                TransactionType = requestItem.TransactionType,
                TransactionDate = requestItem.TransactionDate
            };

            transaction.TransactionLabels = notebookLabels
                .Where(nl => requestItem.Labels.Contains(nl.Id))
                .Select(nl => new TransactionLabel
                {
                    NotebookLabelId = nl.Id
                }).ToList();

            transaction.IsMatched = transaction.TransactionLabels.Any();

            var transactionCreatedDomainEvent = new TransactionCreatedDomainEvent(transaction);
            transaction.AddEvent(transactionCreatedDomainEvent);

            await _transactionRepository.AddAsync(transaction);
        }

        await _transactionRepository.SaveChangesAsync();
        
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}