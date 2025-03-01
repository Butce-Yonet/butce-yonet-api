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

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUsers;

public class GetNotebookUsersQueryHandler : BaseHandler<GetNotebookUsersQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<User, ButceYonetAuthorizationDbContext> _userRepository;
    
    public GetNotebookUsersQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetNotebookUsersQuery request, CancellationToken cancellationToken)
    {
        var currentUserIsMember =  await _notebookUserRepository
            .Get()
            .Where(nu =>
                nu.NotebookId == request.NotebookId &&
                nu.UserId == _user.Id)
            .AnyAsync();

        if (!currentUserIsMember)
            throw new BusinessRuleException("Current user is not notebook user"); //TODO:
        
        var notebookUsers = await
            _notebookUserRepository
                .GetAll()
                .Where(nu => nu.NotebookId == request.NotebookId)
                .ToListAsync();

        var userIds = notebookUsers
            .Select(nu => nu.UserId)
            .ToList();

        var users = await _userRepository
            .GetAll()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        var notebookUserDtos = new List<NotebookUserDto>();

        foreach (var notebookUser in notebookUsers)
        {
            var user = users.FirstOrDefault(u => u.Id == notebookUser.UserId);
            
            if (user is null) 
                continue;
            
            var userDto = _mapper.Map<UserDto>(user);
            var notebookUserDto = new NotebookUserDto
            {
                Id = notebookUser.Id,
                UserId = notebookUser.UserId,
                NotebookId = notebookUser.NotebookId,
                IsDefault = notebookUser.IsDefault,
                User = userDto
            };
            
            notebookUserDtos.Add(notebookUserDto);
        }

        return BaseResponse.Response(notebookUserDtos, HttpStatusCode.OK);
    }
}