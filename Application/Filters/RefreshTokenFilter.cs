using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class RefreshTokenFilter : IFilter<RefreshToken>
{
    public Guid? UserId { get; set; }
    public string? Token { get; set; }

    public IQueryable<RefreshToken> ApplyFilter(IQueryable<RefreshToken> query)
    {
        if (UserId != null)
        {
            query.Where(x => x.UserId == UserId);
        }

        if (!string.IsNullOrEmpty(Token))
        {
            query.Where(x => x.Token == Token);
        }

        return query;
    }

    public string GetCacheKey()
    {
        return $"{nameof(UserId)}={UserId?.ToString() ?? "null"}:" +
            $"{nameof(Token)}={Token ?? "null"}";
    }
}
