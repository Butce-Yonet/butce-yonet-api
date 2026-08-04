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
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;
    private readonly IRecurringTransactionIntervalsService _recurringTransactionIntervalsService;

    public CreateRecurringTransactionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository,
        IRecurringTransactionIntervalsService recurringTransactionIntervalsService)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _userLabelRepository = userLabelRepository;
        _recurringTransactionRepository = recurringTransactionRepository;
        _recurringTransactionIntervalsService = recurringTransactionIntervalsService;
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

        var defaultUserId = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.IsDefault)
            .Select(nu => nu.UserId)
            .FirstOrDefaultAsync();

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == defaultUserId)
            .ToListAsync();

        var matchingLabels = userLabels
            .Where(ul => request.Transaction.Labels.Contains(ul.Id))
            .Select(ul => new TransactionLabelV2 { UserLabelId = ul.Id })
            .ToList();

        var transaction = new TransactionV2
        {
            NotebookId = request.NotebookId,
            ExternalId = "",
            Name = request.Transaction.Name,
            Description = request.Transaction.Description,
            Amount = request.Transaction.Amount,
            CurrencyId = request.Transaction.CurrencyId,
            TransactionType = request.Transaction.TransactionType,
            TransactionDate = request.Transaction.TransactionDate,
            TransactionLabelsV2 = matchingLabels,
            IsMatched = matchingLabels.Any()
        };

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
            StateData = JsonSerializer.Serialize(new List<TransactionV2> { transaction })
        };

        var recurringTransactionAddedDomainEvent = new RecurringTransactionAddedDomainEvent(recurringTransaction);
        recurringTransaction.AddEvent(recurringTransactionAddedDomainEvent);

        await _recurringTransactionRepository.AddAsync(recurringTransaction);
        await _recurringTransactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}