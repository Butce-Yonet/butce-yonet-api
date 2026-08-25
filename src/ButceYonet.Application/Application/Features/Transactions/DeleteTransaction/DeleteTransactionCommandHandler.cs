using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandHandler : BaseHandler<DeleteTransactionCommand, BaseResponse>
{
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;

    public DeleteTransactionCommandHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await
            _transactionRepository
                .Get()
                .Where(t =>
                    t.Id == request.TransactionId &&
                    t.NotebookV2.UserId == _user.Id)
                .Include(t => t.TransactionLabelsV2)
                .FirstOrDefaultAsync();

        if (transaction is null)
            throw new NotFoundException(typeof(TransactionV2));

        if (transaction.TransactionType == TransactionTypes.Saving)
            throw new BusinessRuleException("Birikim işlemleri hedef üzerinden yönetilir, buradan silinemez.");

        var transactionDeletedDomainEvent = new TransactionDeletedDomainEvent(transaction);
        transaction.AddEvent(transactionDeletedDomainEvent);

        transaction.IsDeleted = true;
        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
