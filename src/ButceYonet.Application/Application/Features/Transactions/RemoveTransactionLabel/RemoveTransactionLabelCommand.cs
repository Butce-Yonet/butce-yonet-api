using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.RemoveTransactionLabel;

public class RemoveTransactionLabelCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int TransactionId { get; set; }
    public int LabelId { get; set; }

    public RemoveTransactionLabelCommand()
    {
    }

    public RemoveTransactionLabelCommand(int notebookId, int transactionId, int labelId)
    {
        NotebookId = notebookId;
        TransactionId = transactionId;
        LabelId = labelId;
    }
}