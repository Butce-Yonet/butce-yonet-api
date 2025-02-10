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

namespace ButceYonet.Application.Application.Features.Notebooks.DeleteNotebook;

public class DeleteNotebookCommandHandler : BaseHandler<DeleteNotebookCommand, BaseResponse>
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    
    public DeleteNotebookCommandHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(DeleteNotebookCommand request, CancellationToken cancellationToken)
    {
        var notebook = await
            _notebookRepository
                .Get()
                .Where(p => p.Id == request.Id && !p.IsDefault)
                .Include(p => p.NotebookUsers)
                .FirstOrDefaultAsync();

        if (notebook is null)
            throw new NotFoundException();

        if (!notebook.NotebookUsers.Any(p => p.UserId == _user.Id && p.IsDefault))
            throw new NotFoundException();

        var notebookDeletedDomainEvent = new NotebookDeletedDomainEvent(request.Id);
        
        notebook.AddEvent(notebookDeletedDomainEvent);
        
        _notebookRepository.Remove(notebook);
        await _notebookRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}