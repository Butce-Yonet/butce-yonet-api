using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Interfaces;

public interface IUserPlanValidator
{
    Task<bool> Validate(PlanFeatures feature, IDictionary<string, string> parameters);
}