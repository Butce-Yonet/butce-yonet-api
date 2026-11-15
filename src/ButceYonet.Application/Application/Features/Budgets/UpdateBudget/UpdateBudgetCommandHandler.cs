using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Budgets.UpdateBudget;

public class UpdateBudgetCommandHandler : BaseHandler<UpdateBudgetCommand, BaseResponse>
{
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;

    public UpdateBudgetCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Budget, ButceYonetDbContext> budgetRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _budgetRepository = budgetRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository
            .Get()
            .Where(b => b.Id == request.Id && b.UserId == _user.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (budget is null)
            throw new NotFoundException(typeof(Budget));

        budget.Amount = request.Amount;
        budget.PeriodStartDay = request.PeriodStartDay;

        _budgetRepository.Update(budget);
        await _budgetRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
