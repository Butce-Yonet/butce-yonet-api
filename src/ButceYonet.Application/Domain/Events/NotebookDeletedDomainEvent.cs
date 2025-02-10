using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("notebook-deleted")]
public class NotebookDeletedDomainEvent : IEvent
{
    private Guid _id;

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

    public int NotebookId { get; set; }

    public NotebookDeletedDomainEvent()
    {
    }

    public NotebookDeletedDomainEvent(int notebookId)
    {
        NotebookId = notebookId;
    }
}