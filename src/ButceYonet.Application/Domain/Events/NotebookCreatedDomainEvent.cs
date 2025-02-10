using ButceYonet.Application.Domain.Entities;
using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("notebook-created")]
public class NotebookCreatedDomainEvent : IEvent
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
    
    public Notebook Notebook { get; set; }

    public NotebookCreatedDomainEvent()
    {
    }

    public NotebookCreatedDomainEvent(Notebook notebook)
    {
        Notebook = notebook;
    }
}