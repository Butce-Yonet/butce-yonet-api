using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.NotebookUsers.DeleteNotebookUser;

public class DeleteNotebookUserCommandHandler : BaseHandler<DeleteNotebookUserCommand, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    
    public DeleteNotebookUserCommandHandler(
        ICache cache, 
        IUser user, 
        IMapper mapper, 
        ILocalize localize, 
        IParameterManager parameter, 
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(DeleteNotebookUserCommand request, CancellationToken cancellationToken)
    {
        var isNotebookOwner = await
            _notebookUserRepository
                .Get()
                .Where(nu =>
                    nu.NotebookId == request.NotebookId &&
                    nu.UserId == _user.Id &&
                    nu.IsDefault)
                .AnyAsync();

        if (!isNotebookOwner)
            throw new BusinessRuleException("This notebook user is not default");
        
        var notebookUser = await _notebookUserRepository
            .Get()
            .Where(nu => 
                nu.NotebookId == request.NotebookId &&
                nu.UserId == request.UserId)
            .FirstOrDefaultAsync();

        if (notebookUser is null)
            throw new NotFoundException(typeof(NotebookUser));
        
        if (notebookUser.IsDefault)
            throw new BusinessRuleException("Default notebook user could not be deleted"); //TODO:
        
        var notebookUserDeletedDomainEvent = new NotebookUserDeletedDomainEvent(notebookUser);
        notebookUser.AddEvent(notebookUserDeletedDomainEvent);
        
        _notebookUserRepository.Remove(notebookUser);
        await _notebookUserRepository.SaveChangesAsync();

        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}