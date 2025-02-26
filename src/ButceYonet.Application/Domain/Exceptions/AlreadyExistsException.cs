namespace ButceYonet.Application.Domain.Exceptions;

public class AlreadyExistsException : Exception
{
    public Type EntityType { get; set; }
    
    public AlreadyExistsException(Type entityType) : base("Record Already Exists")
    {
        EntityType = entityType;
    }
}