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

namespace ButceYonet.Application.Application.Features.Notebooks.DeleteNotebook;

public class DeleteNotebookCommandHandler : BaseHandler<DeleteNotebookCommand, BaseResponse>
{
    private readonly IRepository<NotebookV2, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;

    public DeleteNotebookCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<NotebookV2, ButceYonetDbContext> notebookRepository,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _notebookRepository = notebookRepository;
        _transactionRepository = transactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(DeleteNotebookCommand request, CancellationToken cancellationToken)
    {
        var notebook = await
            _notebookRepository
                .Get()
                .Where(p => p.Id == request.Id && p.UserId == _user.Id)
                .FirstOrDefaultAsync();

        if (notebook is null)
            throw new NotFoundException(typeof(NotebookV2));

        var hasTransactions = await _transactionRepository
            .Get()
            .AnyAsync(t => t.NotebookV2Id == notebook.Id);

        if (hasTransactions)
            throw new BusinessRuleException("Bu döneme ait işlem kayıtları bulunduğundan silinemez");

        notebook.IsDeleted = true;
        _notebookRepository.Update(notebook);
        await _notebookRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
