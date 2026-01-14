using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabels;

public class GetNotebookLabelsQueryHandler : BaseHandler<GetNotebookLabelsQuery, BaseResponse>
{
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    
    public GetNotebookLabelsQueryHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(GetNotebookLabelsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyConstants.NotebookLabels.Replace("{NotebookId}", request.NotebookId.ToString());
        var notebookLabelDtos = await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            var notebookLabels = await
                _notebookLabelRepository
                    .GetAll()
                    .Where(nl => nl.NotebookId == request.NotebookId)
                    .OrderBy(nl => nl.Name)
                    .ToListAsync();

            return _mapper.Map<List<NotebookLabelDto>>(notebookLabels);
        }, CacheIntervalConstants.NotebookLabels);

        return BaseResponse.Response(notebookLabelDtos, HttpStatusCode.OK);
    }
}