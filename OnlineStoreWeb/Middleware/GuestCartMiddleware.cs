using Application.Abstractions;
using Application.Dtos.Cart;

namespace OnlineStoreWeb.GuestCartMiddleware;

public class GuestCartMiddleware : IMiddleware
{
    private readonly IGuestCartService _guestCartService;

    public GuestCartMiddleware(IGuestCartService guestCartService)
    {
        _guestCartService = guestCartService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        const string cookieName = "guest_cart_id";
        
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            if (context.Request.Cookies.TryGetValue(cookieName, out var cartIdstr) &&
                Guid.TryParse(cartIdstr, out var cartId))
            {
                CreateGuestCartIdCookie(context, cookieName, cartIdstr);

                await _guestCartService.TouchAsync(cartId);
            }
            else
            {
                var newCart = new GuestCartDto(Guid.NewGuid(), []);
                CreateGuestCartIdCookie(context, cookieName, newCart.Id.ToString());

                await _guestCartService.SaveCartAsync(newCart);
            }
        }

        await next(context);
    }

    private static void CreateGuestCartIdCookie(
        HttpContext context, 
        string cookieName, 
        string newCartId)
    {
        context.Response.Cookies.Append(
            cookieName,
            newCartId,
            new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });
    }
}
