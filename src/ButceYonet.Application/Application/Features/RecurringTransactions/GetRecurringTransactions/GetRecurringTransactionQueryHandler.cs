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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransactions;

public class GetRecurringTransactionQueryHandler : BaseHandler<GetRecurringTransactionsQuery, BaseResponse>
{
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetRecurringTransactionQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Notebook, ButceYonetDbContext> notebookRepository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository,
        IRepository<Currency, ButceYonetDbContext> currencyRepository,
        IHttpContextAccessor httpContextAccessor)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookRepository = notebookRepository;
        _userLabelRepository = userLabelRepository;
        _notebookUserRepository = notebookUserRepository;
        _recurringTransactionRepository = recurringTransactionRepository;
        _currencyRepository = currencyRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetRecurringTransactionsQuery request, CancellationToken cancellationToken)
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

        var paginationRequest = new PaginationFilter(
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageNumber"].ToString()),
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageSize"].ToString()));

        var today = DateTime.UtcNow.Date;

        var recurringTransactions = await
            _recurringTransactionRepository
                .GetAll()
                .Where(rt =>
                    rt.NotebookId == request.NotebookId &&
                    (!rt.EndDate.HasValue || rt.EndDate.Value.Date > today))
                .OrderBy(rt => rt.NextOccurrence ?? DateTime.MaxValue)
                .PaginateAsync(paginationRequest);

        var currencies = await _currencyRepository.GetAll().ToListAsync();

        var defaultUserId = await _notebookUserRepository
            .Get()
            .Where(nu => nu.NotebookId == request.NotebookId && nu.IsDefault)
            .Select(nu => nu.UserId)
            .FirstOrDefaultAsync();

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == defaultUserId)
            .ToListAsync();

        var recurringTransactionDtos = new List<RecurringTransactionDto>();

        foreach (var item in recurringTransactions.Items)
        {
            var transactions = JsonSerializer.Deserialize<List<TransactionV2>>(item.StateData);

            if (!transactions.Any())
                continue;

            var transaction = transactions.FirstOrDefault();
            var currency = currencies.FirstOrDefault(c => c.Id == transaction.CurrencyId);
            var recurringTransactionDto = _mapper.Map<RecurringTransactionDto>(item, opt =>
            {
                opt.Items["Notebook"] = notebook;
                opt.Items["Currency"] = currency;
                opt.Items["UserLabels"] = userLabels;
            });

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