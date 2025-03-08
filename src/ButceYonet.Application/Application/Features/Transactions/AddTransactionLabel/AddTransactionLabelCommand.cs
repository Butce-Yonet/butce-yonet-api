using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.AddTransactionLabel;

public class AddTransactionLabelCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int TransactionId { get; set; }
    public int LabelId { get; set; }

    public AddTransactionLabelCommand()
    {
    }

    public AddTransactionLabelCommand(int notebookId, int transactionId, int labelId)
    {
        NotebookId = notebookId;
        TransactionId = transactionId;
        LabelId = labelId;
    }
}