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

namespace ButceYonet.Application.Application.Features.Notebooks.GetNotebook;

public class GetNotebookQueryHandler : BaseHandler<GetNotebookQuery, BaseResponse>
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    
    public GetNotebookQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetNotebookQuery request, CancellationToken cancellationToken)
    {
        var notebook = await _notebookRepository
            .Get()
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync();

        if (notebook is null)
            throw new NotFoundException(typeof(Notebook));
        
        var notebookDto = _mapper.Map<NotebookDto>(notebook);

        return BaseResponse.Response(notebookDto, HttpStatusCode.OK);
    }
}