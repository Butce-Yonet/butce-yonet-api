using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.NotebookLabels.CreateNotebookLabel;

public class CreateNotebookLabelCommandHandler : BaseHandler<CreateNotebookLabelCommand, BaseResponse>
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    
    public CreateNotebookLabelCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper, 
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Notebook, ButceYonetDbContext> notebookRepository,
        IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateNotebookLabelCommand request, CancellationToken cancellationToken)
    {
        var notebook = await _notebookRepository.GetByIdAsync(request.NotebookId);

        if (notebook is null)
            throw new NotFoundException(typeof(Notebook));

        var userPlanValidatorParameters = new Dictionary<string, string>
        {
            { "NotebookId", request.NotebookId.ToString() }
        };

        await _userPlanValidator.Validate(PlanFeatures.NotebookLabelCount, userPlanValidatorParameters);

        var notebookLabelIsExists = await _notebookLabelRepository
            .Get()
            .AnyAsync(nl => nl.NotebookId == request.NotebookId && nl.Name == request.Name);

        if (notebookLabelIsExists)
            throw new AlreadyExistsException(typeof(NotebookLabel));

        var notebookLabel = new NotebookLabel
        {
            NotebookId = request.NotebookId,
            Name = request.Name,
            ColorCode = request.ColorCode
        };

        await _notebookLabelRepository.AddAsync(notebookLabel);
        await _notebookLabelRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}