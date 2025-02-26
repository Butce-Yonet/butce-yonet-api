namespace ButceYonet.Application.Domain.Exceptions;

public class NotFoundException : Exception
{
    public Type EntityType { get; private set; }
    
    public NotFoundException(Type entityType) : base("Record Not Found")
    {
        EntityType = entityType;
    }
}