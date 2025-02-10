using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Shared.UserPlanRuleValidators;

public class NotebookUserCountRuleValidator : IUserPlanRuleValidator
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;

    public NotebookUserCountRuleValidator(IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository)
    {
        _notebookUserRepository = notebookUserRepository;
    }
    
    public async Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters)
    {
        if (!parameters.ContainsKey("NotebookId"))
            return true;

        if (!int.TryParse(parameters["NotebookId"], out var notebookId))
            return true;

        var notebookUserCount = await _notebookUserRepository
            .GetAll()
            .Where(p => p.NotebookId == notebookId)
            .CountAsync();

        if (planFeature.Count > notebookUserCount)
            return true;
        
        throw new UserPlanValidationException();
    }
}