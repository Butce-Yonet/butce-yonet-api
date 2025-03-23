using ButceYonet.Application.Domain.Entities;
using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("recurring-transaction-created")]
public class RecurringTransactionAddedDomainEvent : IEvent
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

    public RecurringTransaction RecurringTransaction { get; set; }

    public RecurringTransactionAddedDomainEvent()
    {
    }

    public RecurringTransactionAddedDomainEvent(RecurringTransaction recurringTransaction)
    {
        RecurringTransaction = recurringTransaction;
    }
}