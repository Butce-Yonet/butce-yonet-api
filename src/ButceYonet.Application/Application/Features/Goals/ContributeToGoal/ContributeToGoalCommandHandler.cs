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

namespace ButceYonet.Application.Application.Features.Goals.ContributeToGoal;

public class ContributeToGoalCommandHandler : BaseHandler<ContributeToGoalCommand, BaseResponse>
{
    private readonly IRepository<Goal, ButceYonetDbContext> _goalRepository;
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;
    private readonly INotebookPeriodResolver _notebookPeriodResolver;

    public ContributeToGoalCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Goal, ButceYonetDbContext> goalRepository,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository,
        INotebookPeriodResolver notebookPeriodResolver)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _goalRepository = goalRepository;
        _transactionRepository = transactionRepository;
        _notebookPeriodResolver = notebookPeriodResolver;
    }

    public override async Task<BaseResponse> ExecuteRequest(ContributeToGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await _goalRepository
            .Get()
            .Where(g => g.Id == request.GoalId && g.UserId == _user.Id)
            .Include(g => g.GoalLabels.Where(gl => !gl.IsDeleted))
            .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
            throw new NotFoundException(typeof(Goal));

        var contributionDate = request.ContributionDate ?? DateTime.Now;

        var notebook = await _notebookPeriodResolver.ResolveOrCreateAsync(_user.Id, contributionDate, cancellationToken);

        var transaction = new TransactionV2
        {
            NotebookV2Id = notebook.Id,
            GoalId = goal.Id,
            ExternalId = Guid.NewGuid().ToString(),
            Name = goal.Name,
            Description = "",
            Amount = request.Amount,
            CurrencyId = goal.CurrencyId,
            TransactionType = TransactionTypes.Saving,
            TransactionDate = contributionDate,
            TransactionLabelsV2 = goal.GoalLabels
                .Select(gl => new TransactionLabelV2 { UserLabelId = gl.UserLabelId })
                .ToList()
        };
        transaction.IsMatched = transaction.TransactionLabelsV2.Any();

        var transactionCreatedDomainEvent = new TransactionCreatedDomainEvent(transaction);
        transaction.AddEvent(transactionCreatedDomainEvent);

        await _transactionRepository.AddAsync(transaction);

        goal.CurrentAmount += request.Amount;
        _goalRepository.Update(goal);

        await _transactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
