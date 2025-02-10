using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using DotBoil.Caching;
using DotBoil.Localization;
using DotBoil.Parameter;
using MediatR;

namespace ButceYonet.Application.Application.Features;

public abstract class BaseQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly ICache _cache;
    protected readonly IUser _user;
    protected readonly IMapper _mapper;
    protected readonly ILocalize _localize;
    protected readonly IParameterManager _parameter;
    protected readonly IUserPlanValidator _userPlanValidator;

    public BaseQueryHandler(
        ICache cache, 
        IUser user, 
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator)
    {
        _cache = cache;
        _user = user;
        _mapper = mapper;
        _localize = localize;
        _parameter = parameter;
        _userPlanValidator = userPlanValidator;
    }

    public abstract Task<TResponse> ExecuteRequest(TRequest request, CancellationToken cancellationToken);
    
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return await ExecuteRequest(request, cancellationToken);
    }
}