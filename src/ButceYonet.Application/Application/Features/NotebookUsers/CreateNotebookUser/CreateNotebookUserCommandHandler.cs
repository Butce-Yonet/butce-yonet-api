using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.NotebookUsers.CreateNotebookUser;

public class CreateNotebookUserCommandHandler : BaseHandler<CreateNotebookUserCommand, BaseResponse>
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<User, ButceYonetAuthorizationDbContext> _userRepository;
    
    public CreateNotebookUserCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper, 
        ILocalize localize, 
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Notebook, ButceYonetDbContext> notebookRepository,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<User, ButceYonetAuthorizationDbContext> userRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookRepository = notebookRepository;
        _notebookUserRepository = notebookUserRepository;
        _userRepository = userRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateNotebookUserCommand request, CancellationToken cancellationToken)
    {
        var notebook = await _notebookRepository
            .Get()
            .Where(n => n.Id == request.NotebookId)
            .FirstOrDefaultAsync();

        if (notebook is null)
            throw new NotFoundException(typeof(Notebook));

        var notebookUserCountRuleValidatorParameters = new Dictionary<string, string>()
        {
            { "NotebookId", notebook.Id.ToString() }
        };

        await _userPlanValidator.Validate(PlanFeatures.NotebookUserCount, notebookUserCountRuleValidatorParameters);

        var user = await _userRepository
            .Get()
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (user is null)
            throw new NotFoundException(typeof(User));

        var notebookUserIsExists = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.UserId == user.Id)
            .AnyAsync();

        if (notebookUserIsExists)
            throw new AlreadyExistsException(typeof(NotebookUser));

        var isNotebookOwner = await
            _notebookUserRepository
                .Get()
                .Where(nu =>
                    nu.NotebookId == request.NotebookId &&
                    nu.UserId == _user.Id &&
                    nu.IsDefault)
                .AnyAsync();

        if (!isNotebookOwner)
            throw new BusinessRuleException("This notebook user is not default"); //TODO:

        var notebookUser = new NotebookUser
        {
            UserId = user.Id,
            NotebookId = notebook.Id,
            IsDefault = false
        };

        var notebookUserCreatedDomainEvent = new NotebookUserCreatedDomainEvent(notebookUser);

        notebookUser.AddEvent(notebookUserCreatedDomainEvent);

        await _notebookUserRepository.AddAsync(notebookUser);
        await _notebookUserRepository.SaveChangesAsync();
        
        var cacheKey = CacheKeyConstants.NotebookUsers.Replace("{NotebookId}", request.NotebookId.ToString());

        if (await _cache.KeyExistsAsync(cacheKey))
            await _cache.RemoveAsync(cacheKey);
        
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}