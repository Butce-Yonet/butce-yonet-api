using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Interfaces;

public interface IUserPlanRuleValidator
{
    Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters);
}