using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class ResetTokenFilter : IFilter<ResetToken>
{
    public Guid? UserId { get; set; }
    public string? Token { get; set; }

    public IQueryable<ResetToken> ApplyFilter(IQueryable<ResetToken> query)
    {
        if (UserId != null)
        {
            query = query.Where(x => x.UserId == UserId);
        }
        if (!string.IsNullOrEmpty(Token))
        {
            query = query.Where(x => x.Token == Token);
        }
        return query;
    }
}
