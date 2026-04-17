using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQuery : PaginationFilter, IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionTypes? TransactionType { get; set; }
    public List<int> LabelIds { get; set; }

    public GetTransactionsQuery()
    {
        LabelIds = new List<int>();
    }
}