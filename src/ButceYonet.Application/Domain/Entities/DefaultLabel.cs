using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class DefaultLabel : BaseEntity
{
    public string Name { get; set; }
    public string ColorCode { get; set; }
}