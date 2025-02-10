using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Shared.UserPlanRuleValidators;

public class NotebookTransactionCount : IUserPlanRuleValidator
{
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;

    public NotebookTransactionCount(IRepository<Transaction, ButceYonetDbContext> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }
    
    public async Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters)
    {
        if (!parameters.ContainsKey("NotebookId"))
            return true;

        if (!int.TryParse(parameters["NotebookId"], out var notebookId))
            return true;

        var transactionCount = await
            _transactionRepository
                .GetAll()
                .Where(p => p.NotebookId == notebookId)
                .CountAsync();

        if (planFeature.Count > transactionCount)
            return true;
        
        throw new UserPlanValidationException();
    }
}