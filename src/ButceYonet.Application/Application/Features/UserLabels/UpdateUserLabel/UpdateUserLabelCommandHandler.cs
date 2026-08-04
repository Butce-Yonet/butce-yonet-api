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

namespace ButceYonet.Application.Application.Features.UserLabels.UpdateUserLabel;

public class UpdateUserLabelCommandHandler : BaseHandler<UpdateUserLabelCommand, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;

    public UpdateUserLabelCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateUserLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await _userLabelRepository
            .Get()
            .Where(ul => ul.Id == request.Id && ul.UserId == _user.Id)
            .FirstOrDefaultAsync();

        if (label is null)
            throw new NotFoundException(typeof(UserLabel));

        var exists = await _userLabelRepository
            .GetAll()
            .AnyAsync(ul => ul.Id != request.Id && ul.Name == request.Name && (ul.UserId == null || ul.UserId == _user.Id));

        if (exists)
            throw new AlreadyExistsException(typeof(UserLabel));

        label.Name = request.Name;
        label.ColorCode = request.ColorCode;

        _userLabelRepository.Update(label);
        await _userLabelRepository.SaveChangesAsync();

        var cacheKey = CacheKeyConstants.UserLabels.Replace("{UserId}", _user.Id.ToString());
        if (await _cache.KeyExistsAsync(cacheKey))
            await _cache.RemoveAsync(cacheKey);

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}