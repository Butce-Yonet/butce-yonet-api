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

namespace ButceYonet.Application.Application.Features.Subscriptions.MarkSubscriptionAsPaid;

public class MarkSubscriptionAsPaidCommandHandler : BaseHandler<MarkSubscriptionAsPaidCommand, BaseResponse>
{
    private readonly IRepository<Subscription, ButceYonetDbContext> _subscriptionRepository;
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;
    private readonly INotebookPeriodResolver _notebookPeriodResolver;
    private readonly IRecurringTransactionIntervalsService _recurringTransactionIntervalsService;

    public MarkSubscriptionAsPaidCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Subscription, ButceYonetDbContext> subscriptionRepository,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository,
        INotebookPeriodResolver notebookPeriodResolver,
        IRecurringTransactionIntervalsService recurringTransactionIntervalsService)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _subscriptionRepository = subscriptionRepository;
        _transactionRepository = transactionRepository;
        _notebookPeriodResolver = notebookPeriodResolver;
        _recurringTransactionIntervalsService = recurringTransactionIntervalsService;
    }

    public override async Task<BaseResponse> ExecuteRequest(MarkSubscriptionAsPaidCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .Get()
            .Where(s => s.Id == request.SubscriptionId && s.UserId == _user.Id)
            .Include(s => s.SubscriptionLabels.Where(sl => !sl.IsDeleted))
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription is null)
            throw new NotFoundException(typeof(Subscription));

        var paidDate = request.PaidDate ?? DateTime.Now;

        var notebook = await _notebookPeriodResolver.ResolveOrCreateAsync(_user.Id, paidDate, cancellationToken);

        var transaction = new TransactionV2
        {
            NotebookV2Id = notebook.Id,
            ExternalId = Guid.NewGuid().ToString(),
            Name = subscription.Name,
            Description = "",
            Amount = request.Amount,
            CurrencyId = request.CurrencyId,
            TransactionType = TransactionTypes.Expense,
            TransactionDate = paidDate,
            TransactionLabelsV2 = subscription.SubscriptionLabels
                .Select(sl => new TransactionLabelV2 { UserLabelId = sl.UserLabelId })
                .ToList()
        };
        transaction.IsMatched = transaction.TransactionLabelsV2.Any();

        var transactionCreatedDomainEvent = new TransactionCreatedDomainEvent(transaction);
        transaction.AddEvent(transactionCreatedDomainEvent);

        await _transactionRepository.AddAsync(transaction);

        subscription.LastPaidDate = paidDate;
        subscription.LastPaidAmount = request.Amount;
        subscription.NextOccurrence = _recurringTransactionIntervalsService.CalculateInterval(
            subscription.NextOccurrence ?? subscription.StartDate,
            subscription.Frequency,
            subscription.Interval);

        _subscriptionRepository.Update(subscription);

        await _transactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
