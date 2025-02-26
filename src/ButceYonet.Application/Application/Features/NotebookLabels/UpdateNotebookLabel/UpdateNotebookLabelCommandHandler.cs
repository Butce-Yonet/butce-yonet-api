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

namespace ButceYonet.Application.Application.Features.NotebookLabels.UpdateNotebookLabel;

public class UpdateNotebookLabelCommandHandler : BaseHandler<UpdateNotebookLabelCommand, BaseResponse>
{
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    
    public UpdateNotebookLabelCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper,
        ILocalize localize, 
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookLabelRepository = notebookLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateNotebookLabelCommand request, CancellationToken cancellationToken)
    {
        var notebookLabel = await _notebookLabelRepository
            .Get()
            .Where(nl => nl.NotebookId == request.NotebookId && nl.Id == request.NotebookLabelId)
            .FirstOrDefaultAsync();

        if (notebookLabel is null)
            throw new NotFoundException(typeof(NotebookLabel));

        var notebookNameIsUsed = await
            _notebookLabelRepository
                .Get()
                .Where(nl =>
                    nl.NotebookId == request.NotebookId &&
                    nl.Id != request.NotebookLabelId &&
                    nl.Name == request.Name)
                .AnyAsync();

        if (notebookNameIsUsed)
            throw new AlreadyExistsException(typeof(NotebookLabel));

        notebookLabel.Name = request.Name;
        notebookLabel.ColorCode = request.ColorCode;

        _notebookLabelRepository.Update(notebookLabel);
        await _notebookLabelRepository.SaveChangesAsync();
        
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}