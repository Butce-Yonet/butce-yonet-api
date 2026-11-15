using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.UserLabels.DeleteUserLabel;

public class DeleteUserLabelCommandHandler : BaseHandler<DeleteUserLabelCommand, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;

    public DeleteUserLabelCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<Budget, ButceYonetDbContext> budgetRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
        _budgetRepository = budgetRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(DeleteUserLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _userLabelRepository
            .Get()
            .Where(ul => ul.Id == request.Id && ul.UserId == _user.Id)
            .FirstOrDefaultAsync();

        if (label is null)
            throw new NotFoundException(typeof(UserLabel));

        label.IsDeleted = true;
        _userLabelRepository.Update(label);

        // Silinen etikete bağlı bütçeler artık harcama toplayamaz; yetim kalmasınlar.
        var budgets = await _budgetRepository
            .Get()
            .Where(b => b.UserId == _user.Id && b.UserLabelId == label.Id)
            .ToListAsync(cancellationToken);

        foreach (var budget in budgets)
        {
            budget.IsDeleted = true;
            _budgetRepository.Update(budget);
        }

        await _userLabelRepository.SaveChangesAsync();

        var cacheKey = CacheKeyConstants.UserLabels.Replace("{UserId}", _user.Id.ToString());
        if (await _cache.KeyExistsAsync(cacheKey))
            await _cache.RemoveAsync(cacheKey);

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}