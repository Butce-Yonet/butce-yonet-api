using DotBoil.MassTransit.Attributes;
using DotBoil.MessageBroker;

namespace ButceYonet.Application.Domain.Events;

[Queue("user-plan-created")]
public class UserPlanCreatedDomainEvent : IEvent
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

    public int UserId { get; set; }
    public int PlanId { get; set; }
    public DateTime? ExpirationDate { get; set; }
    
    public UserPlanCreatedDomainEvent()
    {
    }

    public UserPlanCreatedDomainEvent(int userId, int planId, DateTime? expirationDate = null)
    {
        UserId = userId;
        PlanId = planId;
        ExpirationDate = expirationDate;
    }
}