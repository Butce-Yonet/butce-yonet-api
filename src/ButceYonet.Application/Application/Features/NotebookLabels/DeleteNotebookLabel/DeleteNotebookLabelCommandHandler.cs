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

namespace ButceYonet.Application.Application.Features.NotebookLabels.DeleteNotebookLabel;

public class DeleteNotebookLabelCommandHandler : BaseHandler<DeleteNotebookLabelCommand, BaseResponse>
{
    private readonly IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    private readonly IRepository<TransactionLabel, ButceYonetDbContext> _transactionLabelRepository;
    
    public DeleteNotebookLabelCommandHandler(
        ICache cache,
        IUser user, 
        IMapper mapper,
        ILocalize localize, 
        IParameterManager parameter, 
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookLabel, ButceYonetDbContext> notebookLabelRepository,
        IRepository<TransactionLabel, ButceYonetDbContext> transactionLabelRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookLabelRepository = notebookLabelRepository;
        _transactionLabelRepository = transactionLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(DeleteNotebookLabelCommand request, CancellationToken cancellationToken)
    {
        var notebookLabel = await _notebookLabelRepository
            .Get()
            .Where(nl => nl.NotebookId == request.NotebookId && nl.Id == request.NotebookLabelId)
            .FirstOrDefaultAsync();

        if (notebookLabel is null)
            throw new NotFoundException(typeof(NotebookLabel));

        var notebookLabelHasTransaction = await
            _transactionLabelRepository
                .Get()
                .Where(tl => tl.NotebookLabelId == request.NotebookLabelId && !tl.Transaction.IsDeleted)
                .AnyAsync();

        if (notebookLabelHasTransaction)
            throw new BusinessRuleException("This label is associated with a transaction"); //TODO:

        _notebookLabelRepository.Remove(notebookLabel);
        await _notebookLabelRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}