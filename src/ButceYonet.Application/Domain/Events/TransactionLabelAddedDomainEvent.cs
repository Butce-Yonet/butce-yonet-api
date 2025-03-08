using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("transaction-label-added")]
public class TransactionLabelAddedDomainEvent : IEvent
{
    private Guid _id = Guid.Empty;

    public Guid Id
    {
        get
        {
            if (_id == Guid.Empty)
                _id = Guid.NewGuid();

            return _id;
        }
        set
        {
            _id = value;
        }
    }
    
    public int TransactionId { get; set; }
    public int NotebookLabelId { get; set; }

    public TransactionLabelAddedDomainEvent()
    {
    }

    public TransactionLabelAddedDomainEvent(int transactionId, int notebookLabelId)
    {
        TransactionId = transactionId;
        NotebookLabelId = notebookLabelId;
    }
}