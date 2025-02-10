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
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    
    public GetNotebooksQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetNotebooksQuery request, CancellationToken cancellationToken)
    {
        var notebooks = await _notebookUserRepository
            .GetAll()
            .Where(p => p.UserId == _user.Id)
            .Include(p => p.Notebook)
            .Select(p => p.Notebook)
            .ToListAsync();
        
        var notebooksDto = _mapper.Map<List<NotebookDto>>(notebooks);
        
        return BaseResponse.Response(notebooksDto, HttpStatusCode.OK);
    }
}