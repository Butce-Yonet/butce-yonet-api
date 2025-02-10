using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Application.Infrastructure.Services;

public class UserPlanValidator : IUserPlanValidator
{
    private readonly IUser _user;
    private readonly ICache _cache;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<UserPlan, ButceYonetDbContext> _userPlanRepository;
    
    public UserPlanValidator(
        IUser user,
        ICache cache,
        IServiceProvider serviceProvider,
        IRepository<UserPlan, ButceYonetDbContext> userPlanRepository)
    {
        _user = user;
        _cache = cache;
        _serviceProvider = serviceProvider;
        _userPlanRepository = userPlanRepository;
    }
    
    public async Task<bool> Validate(PlanFeatures feature, IDictionary<string, string> parameters)
    {
        var userId = _user.Id;
        
        var cacheKey = CacheKeyConstants.CurrentUserPlan.Replace("{UserId}", userId.ToString());
        
        var userPlanFeatures = await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            var userPlan = await _userPlanRepository
                .Get()
                .Where(up => up.UserId == userId)
                .OrderByDescending(p => p.Id)
                .Include(p => p.Plan)
                .ThenInclude(p => p.PlanFeatures)
                .FirstOrDefaultAsync();

            if (userPlan is null)
                return Enumerable.Empty<PlanFeature>();

            if (userPlan.ExpirationDate.HasValue && userPlan.ExpirationDate < DateTime.Now)
                return Enumerable.Empty<PlanFeature>();

            return userPlan.Plan.PlanFeatures;
        }, CacheIntervalConstants.CurrentUserPlan);

        var planFeature = userPlanFeatures.FirstOrDefault(upf => upf.Feature == feature);

        if (planFeature is null)
            return true;

        using var scope = _serviceProvider.CreateScope();
        var ruleValidator = scope.ServiceProvider.GetKeyedService<IUserPlanRuleValidator>(planFeature.Code);

        if (ruleValidator is null)
            return true;

        return await ruleValidator.Validate(userId, planFeature, parameters);
    }
}