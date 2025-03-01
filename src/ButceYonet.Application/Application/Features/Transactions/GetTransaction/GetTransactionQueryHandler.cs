using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQueryHandler : BaseHandler<GetTransactionQuery, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    
    public GetTransactionQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<Transaction, ButceYonetDbContext> transactionRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _transactionRepository = transactionRepository;
    }

    public override Task<BaseResponse> ExecuteRequest(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}