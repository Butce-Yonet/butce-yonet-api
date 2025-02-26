using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
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

namespace ButceYonet.Application.Application.Features.Notebooks.CreateNotebook;

public class CreateNotebookCommandHandler : BaseHandler<CreateNotebookCommand, BaseResponse>
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    
    public CreateNotebookCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper, 
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Notebook, ButceYonetDbContext> notebookRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookRepository = notebookRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateNotebookCommand request, CancellationToken cancellationToken)
    {
        await _userPlanValidator.Validate(PlanFeatures.NotebookCount, new Dictionary<string, string>());

        var notebookIsExists = await _notebookRepository
            .Get()
            .AnyAsync(n => n.Name == request.Name);

        if (notebookIsExists)
            throw new AlreadyExistsException();
        
        var notebook = new Notebook
        {
            Name = request.Name,
            IsDefault = false
        };

        var notebookUser = new NotebookUser
        {
            UserId = _user.Id,
            IsDefault = true
        };

        notebook.NotebookUsers.Add(notebookUser);
        
        var notebookCreatedDomainEvent = new NotebookCreatedDomainEvent
        {
            Notebook = notebook
        };

        notebook.AddEvent(notebookCreatedDomainEvent);

        await _notebookRepository.AddAsync(notebook);
        await _notebookRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}