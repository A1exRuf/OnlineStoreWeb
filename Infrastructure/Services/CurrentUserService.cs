using Application.Abstractions;
using Domain.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (Guid.TryParse(id, out var result))
                return result;

            return null;
        }
    }

    public string? Email
    {
        get
        {
            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

            return email;
        }
    }

    public UserRole? Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

            if(Enum.TryParse<UserRole>(role, true, out var result))
                return result;

            return null;
        }
    }

    public Guid GuestCartId
    {
        get
        {
            IRequestCookieCollection cookies = _httpContextAccessor.HttpContext?.Request.Cookies!;

            cookies.TryGetValue("guest_cart_id", out var cartIdstr);
            Guid.TryParse(cartIdstr, out var cartId);

            return cartId;
        }
    }
}