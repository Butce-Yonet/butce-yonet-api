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

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQueryHandler : BaseHandler<GetTransactionQuery, BaseResponse>
{
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;

    public GetTransactionQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _transactionRepository = transactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await
            _transactionRepository
                .Get()
                .Where(t =>
                    t.Id == request.TransactionId &&
                    t.NotebookV2.UserId == _user.Id)
                .Include(t => t.NotebookV2)
                .Include(t => t.Currency)
                .Include(t => t.TransactionLabelsV2)
                .ThenInclude(tl => tl.UserLabel)
                .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException(typeof(TransactionV2));

        var responseDto = _mapper.Map<TransactionDto>(transaction);

        return BaseResponse.Response(responseDto, HttpStatusCode.OK);
    }
}
