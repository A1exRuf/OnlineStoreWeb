using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class RefreshTokenFilter : IFilter<RefreshToken>
{
    public Guid? UserId { get; set; }

    public IQueryable<RefreshToken> ApplyFilter(IQueryable<RefreshToken> query)
    {
        if (UserId != null)
        {
            query.Where(x => x.UserId == UserId);
        }

        return query;
    }
}
