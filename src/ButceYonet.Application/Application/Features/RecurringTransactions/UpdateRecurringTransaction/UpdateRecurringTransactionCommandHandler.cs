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

namespace ButceYonet.Application.Application.Features.RecurringTransactions.UpdateRecurringTransaction;

public class UpdateRecurringTransactionCommandHandler : BaseHandler<UpdateRecurringTransactionCommand, BaseResponse>
{
    private readonly IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private readonly IRepository<RecurringTransaction, ButceYonetDbContext> _recurringTransactionRepository;
    
    public UpdateRecurringTransactionCommandHandler(
        ICache cache, 
        IUser user, 
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter, 
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookUser, ButceYonetDbContext> notebookUserRepository,
        IRepository<RecurringTransaction, ButceYonetDbContext> recurringTransactionRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookUserRepository = notebookUserRepository;
        _recurringTransactionRepository = recurringTransactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var isNotebookUser = await
            _notebookUserRepository
                .Get()
                .Where(nu =>
                    nu.NotebookId == request.NotebookId &&
                    nu.UserId == _user.Id)
                .AnyAsync();

        if (!isNotebookUser)
            throw new BusinessRuleException("User is not in notebook"); //TODO:

        var recurringTransaction = await _recurringTransactionRepository
            .Get()
            .Where(rt => rt.Id == request.Id && rt.NotebookId == request.NotebookId)
            .FirstOrDefaultAsync();

        if (recurringTransaction is null)
            throw new NotFoundException(typeof(RecurringTransaction));

        recurringTransaction.Name = request.Name;
        recurringTransaction.Description = request.Description;
        recurringTransaction.StartDate = request.StartDate;
        recurringTransaction.EndDate = request.EndDate;
        recurringTransaction.Frequency = request.Frequency;
        recurringTransaction.Interval = request.Interval;
        recurringTransaction.NextOccurrence = request.NextOccurence;
        
        _recurringTransactionRepository.Update(recurringTransaction);
        await _recurringTransactionRepository.SaveChangesAsync();
        
        return BaseResponse.Response(new {}, HttpStatusCode.OK);
    }
}