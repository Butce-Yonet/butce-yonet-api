namespace ButceYonet.Application.Domain.Exceptions;

public class UserPlanValidationException : Exception
{
    public UserPlanValidationException() : base("Kullanıcı planınızı yükseltiniz")
    {
        
    }   
}