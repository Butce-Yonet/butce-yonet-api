using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
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
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;
    private readonly INotebookPeriodResolver _notebookPeriodResolver;

    public CreateTransactionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository,
        INotebookPeriodResolver notebookPeriodResolver)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
        _transactionRepository = transactionRepository;
        _notebookPeriodResolver = notebookPeriodResolver;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
            .ToListAsync(cancellationToken);

        var validatedNotebookIds = new HashSet<int>();

        foreach (var requestItem in request.Transactions)
        {
            var notebook = await _notebookPeriodResolver.ResolveOrCreateAsync(_user.Id, requestItem.TransactionDate, cancellationToken);

            if (validatedNotebookIds.Add(notebook.Id))
            {
                var notebookTransactionCountValidateParameters = new Dictionary<string, string>
                {
                    { "NotebookId", notebook.Id.ToString() }
                };

                await _userPlanValidator.Validate(PlanFeatures.NotebookTransactionCount, notebookTransactionCountValidateParameters);
            }

            var matchingLabelIds = userLabels
                .Where(ul => requestItem.Labels.Contains(ul.Id))
                .Select(ul => ul.Id)
                .ToList();

            var transaction = new TransactionV2
            {
                NotebookV2Id = notebook.Id,
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
