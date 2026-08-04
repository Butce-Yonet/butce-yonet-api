using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.UserLabels.GetUserLabel;

public class GetUserLabelQueryHandler : BaseHandler<GetUserLabelQuery, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;

    public GetUserLabelQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetUserLabelQuery request, CancellationToken cancellationToken)
    {
        var label = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.Id == request.Id && (ul.UserId == null || ul.UserId == _user.Id))
            .FirstOrDefaultAsync();

        if (label is null)
            throw new NotFoundException(typeof(UserLabel));

        var dto = _mapper.Map<UserLabelDto>(label);

        return BaseResponse.Response(dto, HttpStatusCode.OK);
    }
}