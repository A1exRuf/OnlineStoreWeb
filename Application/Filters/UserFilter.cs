using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters
{
    public class UserFilter : IFilter<User>
    {
        public Guid? Guid { get; set; }
        public string? Email { get; set; }

        public IQueryable<User> ApplyFilter(IQueryable<User> query)
        {
            if (Guid.HasValue)
            {
                query = query.Where(x => x.Id == Guid.Value);

                return query;
            }

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(x => x.Email == Email);
            }

            return query;
        }
    }
}
