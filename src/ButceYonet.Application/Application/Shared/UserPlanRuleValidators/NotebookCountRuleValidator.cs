using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;

namespace ButceYonet.Application.Application.Shared.UserPlanRuleValidators;

public class NotebookCountRuleValidator : IUserPlanRuleValidator
{
    public async Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters)
    {
        return true;
    }
}