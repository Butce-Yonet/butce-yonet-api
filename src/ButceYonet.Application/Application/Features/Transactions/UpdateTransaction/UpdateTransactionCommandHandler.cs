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

namespace ButceYonet.Application.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler : BaseHandler<UpdateTransactionCommand, BaseResponse>
{
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;
    private readonly IRepository<TransactionLabelV2, ButceYonetDbContext> _transactionLabelV2Repository;
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly INotebookPeriodResolver _notebookPeriodResolver;

    public UpdateTransactionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository,
        IRepository<TransactionLabelV2, ButceYonetDbContext> transactionLabelV2Repository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        INotebookPeriodResolver notebookPeriodResolver)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _transactionRepository = transactionRepository;
        _transactionLabelV2Repository = transactionLabelV2Repository;
        _userLabelRepository = userLabelRepository;
        _notebookPeriodResolver = notebookPeriodResolver;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await
            _transactionRepository
                .Get()
                .Where(t =>
                    t.Id == request.TransactionId &&
                    t.NotebookV2.UserId == _user.Id)
                .Include(t => t.NotebookV2)
                .Include(t => t.TransactionLabelsV2)
                .FirstOrDefaultAsync(cancellationToken);

        if (transaction is null)
            throw new NotFoundException(typeof(TransactionV2));

        var currentTermStart = new DateTime(transaction.NotebookV2.TermStart.Year, transaction.NotebookV2.TermStart.Month, 1);
        var newTermStart = new DateTime(request.TransactionDate.Year, request.TransactionDate.Month, 1);

        if (newTermStart != currentTermStart)
        {
            var newNotebook = await _notebookPeriodResolver.ResolveOrCreateAsync(_user.Id, request.TransactionDate, cancellationToken);
            transaction.NotebookV2Id = newNotebook.Id;
        }

        transaction.Name = request.Name;
        transaction.Description = request.Description;
        transaction.Amount = request.Amount;
        transaction.CurrencyId = request.CurrencyId;
        transaction.TransactionType = request.TransactionType;
        transaction.TransactionDate = request.TransactionDate;

        foreach (var label in transaction.TransactionLabelsV2)
        {
            label.IsDeleted = true;
            _transactionLabelV2Repository.Update(label);
        }

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
            .ToListAsync(cancellationToken);

        var matchingLabelIds = userLabels
            .Where(ul => request.Labels.Contains(ul.Id))
            .Select(ul => ul.Id)
            .ToList();

        foreach (var labelId in matchingLabelIds)
        {
            await _transactionLabelV2Repository.AddAsync(new TransactionLabelV2
            {
                TransactionV2Id = request.TransactionId,
                UserLabelId = labelId
            });
        }

        transaction.IsMatched = matchingLabelIds.Any();

        var oldTransaction = await
            _transactionRepository
                .Get()
                .Where(t => t.Id == request.TransactionId)
                .Include(t => t.TransactionLabelsV2)
                .FirstOrDefaultAsync(cancellationToken);

        var transactionUpdatedDomainEvent = new TransactionUpdatedDomainEvent();
        transactionUpdatedDomainEvent.OldTransaction = oldTransaction;
        transactionUpdatedDomainEvent.NewTransaction = transaction;

        transaction.AddEvent(transactionUpdatedDomainEvent);

        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
