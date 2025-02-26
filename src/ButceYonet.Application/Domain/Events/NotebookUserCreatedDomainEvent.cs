using ButceYonet.Application.Domain.Entities;
using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("notebook-user-created")]
public class NotebookUserCreatedDomainEvent : IEvent
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

    public NotebookUser NotebookUser { get; set; }

    public NotebookUserCreatedDomainEvent()
    {
    }

    public NotebookUserCreatedDomainEvent(NotebookUser notebookUser)
    {
        NotebookUser = notebookUser;
    }
}