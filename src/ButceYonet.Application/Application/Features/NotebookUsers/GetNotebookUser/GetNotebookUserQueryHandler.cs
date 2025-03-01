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

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUser;

public class GetNotebookUserQueryHandler : BaseHandler<GetNotebookUserQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<User, ButceYonetAuthorizationDbContext> _userRepository;
    
    public GetNotebookUserQueryHandler(
        ICache cache, 
        IUser user,
        IMapper mapper, 
        ILocalize localize, 
        IParameterManager parameter, 
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<User, ButceYonetAuthorizationDbContext> userRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _userRepository = userRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetNotebookUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserIsMember =  await _notebookUserRepository
            .Get()
            .Where(nu =>
                nu.NotebookId == request.NotebookId &&
                nu.UserId == _user.Id)
            .AnyAsync();

        if (!currentUserIsMember)
            throw new BusinessRuleException("Current user is not notebook user"); //TODO:

        var notebookUser = await
            _notebookUserRepository
                .Get()
                .Where(nu => nu.NotebookId == request.NotebookId &&
                             nu.Id == request.Id)
                .FirstOrDefaultAsync();
        
        if (notebookUser is null)
            throw new NotFoundException(typeof(NotebookUser));

        var user = await _userRepository
            .Get()
            .Where(u => u.Id == notebookUser.UserId)
            .FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(typeof(User));

        var userDto = _mapper.Map<UserDto>(user);

        var notebookUserDto = new NotebookUserDto
        {
            Id = notebookUser.Id,
            UserId = notebookUser.UserId,
            NotebookId = notebookUser.NotebookId,
            IsDefault = notebookUser.IsDefault,
            User = userDto
        };

        return BaseResponse.Response(notebookUserDto, HttpStatusCode.OK);
    }
}