using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Shared.UserPlanRuleValidators;

public class NotebookCountRuleValidator : IUserPlanRuleValidator
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;

    public NotebookCountRuleValidator(IRepository<Notebook, ButceYonetDbContext> notebookRepository)
    {
        _notebookRepository = notebookRepository;
    }
    
    public async Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters)
    {
        var notebookCount = await _notebookRepository
            .GetAll()
            .Where(u => u.NotebookUsers.Any(y => y.UserId == userId))
            .CountAsync();
        
        if (planFeature.Count > notebookCount)
            return true;
        
        throw new UserPlanValidationException();
    }
}