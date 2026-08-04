using ButceYonet.Application.Domain.Entities;
using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("transaction-created")]
public class TransactionCreatedDomainEvent : IEvent
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

    public TransactionV2 Transaction { get; set; }

    public TransactionCreatedDomainEvent()
    {
    }

    public TransactionCreatedDomainEvent(TransactionV2 transaction)
    {
        Transaction = transaction;
    }
}