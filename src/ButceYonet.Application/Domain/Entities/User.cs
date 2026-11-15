using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string ActiveLanguage { get; set; }
    public bool IsPassive { get; set; }
}