namespace ButceYonet.Application.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base("Record Not Found")
    {
        
    }
}