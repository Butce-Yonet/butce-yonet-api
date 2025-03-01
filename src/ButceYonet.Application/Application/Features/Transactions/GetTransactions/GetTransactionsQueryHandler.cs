using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using DotBoil.Caching;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryHandler : BaseHandler<GetTransactionsQuery, BaseResponse>
{
    public GetTransactionsQueryHandler(
        ICache cache, 
        IUser user, 
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
    }

    public override Task<BaseResponse> ExecuteRequest(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}