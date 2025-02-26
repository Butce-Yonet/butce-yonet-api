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

namespace ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabel;

public class GetNoteboolLabelQueryHandler : BaseHandler<GetNotebookLabelQuery, BaseResponse>
{
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    
    public GetNoteboolLabelQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetNotebookLabelQuery request, CancellationToken cancellationToken)
    {
        var notebookLabel = await
            _notebookLabelRepository
                .Get()
                .Where(nl => nl.NotebookId == request.NotebookId && nl.Id == request.NotebookLabelId)
                .FirstOrDefaultAsync();

        if (notebookLabel is null)
            throw new NotFoundException();
        
        var notebookLabelDto = _mapper.Map<NotebookLabelDto>(notebookLabel);
        
        return BaseResponse.Response(notebookLabelDto, HttpStatusCode.OK);
    }
}