using System.Net;
using System.Text.Json;
using AutoMapper;
using ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransactions;

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

        var notebook = await
            _notebookRepository
                .Get()
                .Where(n => n.Id == request.NotebookId)
                .FirstOrDefaultAsync();

        if (notebook is null)
            throw new BusinessRuleException(""); //TODO:

        var recurringTransactions = await
            _recurringTransactionRepository
                .GetAll()
                .Where(rt => rt.NotebookId == request.NotebookId)
                .PaginateAsync(request);

        var currencies = await _currencyRepository.GetAll().ToListAsync();
        var notebookLabels = await _notebookLabelRepository.GetAll().Where(nl => nl.NotebookId == request.NotebookId).ToListAsync();

        var recurringTransactionDtos = new List<RecurringTransactionDto>();

        foreach (var item in recurringTransactions.Items)
        {
            var transaction = JsonSerializer.Deserialize<Transaction>(item.StateData);
            var currency = currencies.Where(c => c.Id == transaction.CurrencyId).FirstOrDefault();
            var recurringTransactionDto = _mapper.Map<RecurringTransactionDto>(item, opt =>
            {
                opt.Items["Notebook"] = notebook;
                opt.Items["Currency"] = currency;
                opt.Items["NotebookLabels"] = notebookLabels;
            } );
            
            recurringTransactionDtos.Add(recurringTransactionDto);
        }
        
        return BaseResponse.Response(new PaginatedModel<RecurringTransactionDto>(
            recurringTransactions.PageNumber, 
            recurringTransactions.PageSize,
            recurringTransactions.TotalPages,
            recurringTransactions.TotalRecords,
            recurringTransactionDtos), HttpStatusCode.OK);
    }
}