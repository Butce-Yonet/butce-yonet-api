using System.Net;
using System.Text.Json;
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

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;

public class GetRecurringTransactionQueryHandler : BaseHandler<GetRecurringTransactionQuery, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;

    public GetRecurringTransactionQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository,
        IRepository<Currency, ButceYonetDbContext> currencyRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
        _recurringTransactionRepository = recurringTransactionRepository;
        _currencyRepository = currencyRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetRecurringTransactionQuery request, CancellationToken cancellationToken)
    {
        var recurringTransaction = await
            _recurringTransactionRepository
                .Get()
                .Where(rt =>
                    rt.UserId == _user.Id &&
                    rt.Id == request.RecurringTransactionId &&
                    (!rt.EndDate.HasValue || rt.EndDate >= DateTime.Now))
                .FirstOrDefaultAsync();

        if (recurringTransaction is null)
            throw new NotFoundException(typeof(RecurringTransaction));

        var transactions = JsonSerializer.Deserialize<List<TransactionV2>>(recurringTransaction.StateData);
        var transaction = transactions?.FirstOrDefault();

        if (transaction is null)
            throw new NotFoundException(typeof(TransactionV2));

        var currency = await
            _currencyRepository
                .Get()
                .Where(c => c.Id == transaction.CurrencyId)
                .FirstOrDefaultAsync();

        if (currency is null)
            throw new NotFoundException(typeof(Currency));

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
            .ToListAsync();

        var recurringTransactionDto = _mapper.Map<RecurringTransactionDto>(recurringTransaction, opt =>
        {
            opt.Items["Currency"] = currency;
            opt.Items["UserLabels"] = userLabels;
        });

        return BaseResponse.Response(recurringTransactionDto, HttpStatusCode.OK);
    }
}
