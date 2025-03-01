using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQuery : PaginationFilter, IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}