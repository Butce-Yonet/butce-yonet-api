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
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;
    private readonly IRepository<TransactionLabelV2, ButceYonetDbContext> _transactionLabelV2Repository;
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;

    public UpdateTransactionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository,
        IRepository<TransactionLabelV2, ButceYonetDbContext> transactionLabelV2Repository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _transactionRepository = transactionRepository;
        _transactionLabelV2Repository = transactionLabelV2Repository;
        _userLabelRepository = userLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateTransactionCommand request, CancellationToken cancellationToken)
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
                .Include(t => t.TransactionLabelsV2)
                .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException(typeof(Transaction));

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

        var defaultUserId = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.IsDefault)
            .Select(nu => nu.UserId)
            .FirstOrDefaultAsync();

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == defaultUserId)
            .ToListAsync();

        var matchingLabelIds = userLabels
            .Where(ul => request.Labels.Contains(ul.Id))
            .Select(ul => ul.Id)
            .ToList();

        foreach (var labelId in matchingLabelIds)
        {
            await _transactionLabelV2Repository.AddAsync(new TransactionLabelV2
            {
                TransactionId = request.TransactionId,
                UserLabelId = labelId
            });
        }

        transaction.IsMatched = matchingLabelIds.Any();

        var oldTransaction = await
            _transactionRepository
                .Get()
                .Where(t =>
                    t.NotebookId == request.NotebookId &&
                    t.Id == request.TransactionId)
                .Include(t => t.TransactionLabelsV2)
                .FirstOrDefaultAsync();

        var transactionUpdatedDomainEvent = new TransactionUpdatedDomainEvent();
        transactionUpdatedDomainEvent.OldTransaction = oldTransaction;
        transactionUpdatedDomainEvent.NewTransaction = transaction;

        transaction.AddEvent(transactionUpdatedDomainEvent);

        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}