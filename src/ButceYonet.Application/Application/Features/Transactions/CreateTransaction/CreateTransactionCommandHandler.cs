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
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;

    public CreateTransactionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
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
            { "NotebookId", request.NotebookId.ToString() }
        };

        await _userPlanValidator.Validate(PlanFeatures.NotebookTransactionCount, notebookTransactionCountValidateParameters);

        var defaultUserId = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.IsDefault)
            .Select(nu => nu.UserId)
            .FirstOrDefaultAsync();

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == defaultUserId)
            .ToListAsync();

        var pendingLabels = new List<(Transaction transaction, List<int> labelIds)>();

        foreach (var requestItem in request.Transactions)
        {
            var matchingLabelIds = userLabels
                .Where(ul => requestItem.Labels.Contains(ul.Id))
                .Select(ul => ul.Id)
                .ToList();

            var transaction = new TransactionV2
            {
                NotebookId = request.NotebookId,
                ExternalId = Guid.NewGuid().ToString(),
                Name = requestItem.Name,
                Description = requestItem.Description,
                Amount = requestItem.Amount,
                CurrencyId = requestItem.CurrencyId,
                TransactionType = requestItem.TransactionType,
                TransactionDate = requestItem.TransactionDate,
                IsMatched = matchingLabelIds.Any(),
                TransactionLabelsV2 = matchingLabelIds.Select(ul => new TransactionLabelV2()
                {
                    UserLabelId = ul
                }).ToList()
            };

            var transactionCreatedDomainEvent = new TransactionCreatedDomainEvent(transaction);
            transaction.AddEvent(transactionCreatedDomainEvent);

            await _transactionRepository.AddAsync(transaction);
        }

        await _transactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}