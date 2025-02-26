using ButceYonet.Application.Domain.Entities;
using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("notebook-user-deleted")]
public class NotebookUserDeletedDomainEvent : IEvent
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

    public NotebookUser NotebookUser { get; set; }

    public NotebookUserDeletedDomainEvent()
    {
    }

    public NotebookUserDeletedDomainEvent(NotebookUser notebookUser)
    {
        NotebookUser = notebookUser;
    }
}