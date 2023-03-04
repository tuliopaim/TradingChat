using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TradingChat.Domain.Contracts;

namespace TradingChat.Infrastructure;
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _acessor;

    public CurrentUser(IHttpContextAccessor acessor)
    {
        _acessor = acessor;
    }

    public Guid? Id => ObterId();

    private Guid? ObterId()
    {
        var idClaim = _acessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

        return Guid.TryParse(idClaim?.Value, out var guidId)
            ? guidId
            : null;
    }
}
