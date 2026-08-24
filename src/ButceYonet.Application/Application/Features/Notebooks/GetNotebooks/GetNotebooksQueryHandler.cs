using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Notebooks.GetNotebooks;

public class GetNotebooksQueryHandler : BaseHandler<GetNotebooksQuery, BaseResponse>
{
    private readonly IRepository<NotebookV2, ButceYonetDbContext> _notebookRepository;

    public GetNotebooksQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookV2, ButceYonetDbContext> notebookRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookRepository = notebookRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetNotebooksQuery request, CancellationToken cancellationToken)
    {
        var notebooks = await _notebookRepository
            .Get()
            .Where(p => p.UserId == _user.Id)
            .OrderByDescending(p => p.TermStart)
            .ToListAsync();

        var notebooksDto = _mapper.Map<List<NotebookDto>>(notebooks);

        return BaseResponse.Response(notebooksDto, HttpStatusCode.OK);
    }
}
