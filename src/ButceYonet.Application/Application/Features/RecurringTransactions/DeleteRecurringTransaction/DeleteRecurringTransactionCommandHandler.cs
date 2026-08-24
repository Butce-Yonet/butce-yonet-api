using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommandHandler : BaseHandler<DeleteRecurringTransactionCommand, BaseResponse>
{
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;

    public DeleteRecurringTransactionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _recurringTransactionRepository = recurringTransactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(DeleteRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var recurringTransaction = await
            _recurringTransactionRepository
                .Get()
                .Where(rt =>
                    rt.UserId == _user.Id &&
                    rt.Id == request.RecurringTransactionId)
                .FirstOrDefaultAsync();

        if (recurringTransaction is null)
            throw new NotFoundException(typeof(RecurringTransaction));

        recurringTransaction.IsDeleted = true;
        _recurringTransactionRepository.Update(recurringTransaction);
        await _recurringTransactionRepository.SaveChangesAsync();

        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}
