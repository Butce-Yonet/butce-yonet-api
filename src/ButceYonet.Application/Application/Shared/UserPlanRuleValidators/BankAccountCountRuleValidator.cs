using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Shared.UserPlanRuleValidators;

public class BankAccountCountRuleValidator : IUserPlanRuleValidator
{
    private readonly IRepository<BankAccount, ButceYonetDbContext> _bankAccountRepository;

    public BankAccountCountRuleValidator(IRepository<BankAccount, ButceYonetDbContext> bankAccountRepository)
    {
        _bankAccountRepository = bankAccountRepository;
    }
    
    public async Task<bool> Validate(int userId, PlanFeature planFeature, IDictionary<string, string> parameters)
    {
        var bankAccountCount = await _bankAccountRepository
            .GetAll()
            .Where(p => p.UserId == userId)
            .CountAsync();

        if (planFeature.Count > bankAccountCount)
            return true;
        
        throw new UserPlanValidationException();
    }
}