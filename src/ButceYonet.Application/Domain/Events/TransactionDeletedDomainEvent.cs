using ButceYonet.Application.Domain.Entities;
using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("transaction-deleted")]
public class TransactionDeletedDomainEvent : IEvent
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

    public Transaction Transaction { get; set; }

    public TransactionDeletedDomainEvent()
    {
    }

    public TransactionDeletedDomainEvent(Transaction transaction)
    {
        Transaction = transaction;
    }
}