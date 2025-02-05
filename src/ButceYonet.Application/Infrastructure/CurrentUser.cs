using DotBoil.EFCore;

namespace ButceYonet.Application.Infrastructure;

public class CurrentUser : IAuditUser
{
    public async Task<string> GetModifierName()
    {
        return "";
    }
}