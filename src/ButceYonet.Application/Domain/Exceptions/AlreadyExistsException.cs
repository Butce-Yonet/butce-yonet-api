namespace ButceYonet.Application.Domain.Exceptions;

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException() : base("Record Already Exists")
    {
        
    }
}