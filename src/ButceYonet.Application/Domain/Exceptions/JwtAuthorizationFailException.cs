namespace ButceYonet.Application.Domain.Exceptions;

public class JwtAuthorizationFailException : Exception
{
    public JwtAuthorizationFailException() : base ("Unauthorized")
    {
        
    }
}