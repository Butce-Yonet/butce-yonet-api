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
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;
    
    public GetRecurringTransactionQueryHandler(
        ICache cache, 
        IUser user, 
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Notebook, ButceYonetDbContext> notebookRepository,
        IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository,
        IRepository<Currency, ButceYonetDbContext> currencyRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookRepository = notebookRepository;
        _notebookLabelRepository = notebookLabelRepository;
        _notebookUserRepository = notebookUserRepository;
        _recurringTransactionRepository = recurringTransactionRepository;
        _currencyRepository = currencyRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetRecurringTransactionQuery request, CancellationToken cancellationToken)
    {
        var isNotebookUser = await
            _notebookUserRepository
                .Get()
                .Where(nu =>
                    nu.NotebookId == request.NotebookId &&
                    nu.UserId == _user.Id)
                .AnyAsync();

        if (!isNotebookUser)
            throw new BusinessRuleException(""); //TODO:

        var recurringTransaction = await
            _recurringTransactionRepository
                .Get()
                .Where(rt =>
                    rt.NotebookId == request.NotebookId &&
                    rt.Id == request.RecurringTransactionId &&
                    (!rt.EndDate.HasValue || rt.EndDate >= DateTime.Now))
                .FirstOrDefaultAsync();

        if (recurringTransaction is null)
            throw new NotFoundException(typeof(RecurringTransaction));
        
        var notebook = await 
            _notebookRepository
                .Get()
                .Where(n => 
                    n.Id == request.NotebookId)
                .FirstOrDefaultAsync();
        
        if (notebook is null)
            throw new NotFoundException(typeof(Notebook));

        var transaction = JsonSerializer.Deserialize<Transaction>(recurringTransaction.StateData);

        var currency = await
            _currencyRepository
                .Get()
                .Where(c => c.Id == transaction.CurrencyId)
                .FirstOrDefaultAsync();
        
        if (currency is null)
            throw new NotFoundException(typeof(Currency));
        
        var notebookLabels = await
            _notebookLabelRepository
                .GetAll()
                .Where(nl => nl.NotebookId == request.NotebookId)
                .ToListAsync();

        var recurringTransactionDto = _mapper.Map<RecurringTransactionDto>(recurringTransaction, opt =>
        {
            opt.Items["Notebook"] = notebook;
            opt.Items["Currency"] = currency;
            opt.Items["NotebookLabels"] = notebookLabels;
        });
            
        return BaseResponse.Response(recurringTransactionDto, HttpStatusCode.OK);
    }
}