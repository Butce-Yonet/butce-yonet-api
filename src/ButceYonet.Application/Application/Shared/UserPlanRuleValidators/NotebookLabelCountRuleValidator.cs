using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Shared.UserPlanRuleValidators;

public class NotebookLabelCountRuleValidator : IUserPlanRuleValidator
{
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;

    public NotebookLabelCountRuleValidator(IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository)
    {
        _notebookLabelRepository = notebookLabelRepository;
    }
    
    public async Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters)
    {
        if (!parameters.ContainsKey("NotebookId"))
            return true;

        if (!int.TryParse(parameters["NotebookId"], out var notebookId))
            return true;

        var notebookLabelCound = await _notebookLabelRepository
            .GetAll()
            .Where(p => p.NotebookId == notebookId)
            .CountAsync();

        if (planFeature.Count > notebookLabelCound)
            return true;
        
        throw new UserPlanValidationException();
    }
}