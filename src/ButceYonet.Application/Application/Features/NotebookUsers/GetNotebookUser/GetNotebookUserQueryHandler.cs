using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUser;

public class GetNotebookUserQueryHandler : BaseHandler<GetNotebookUserQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    
    public GetNotebookUserQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetNotebookUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}