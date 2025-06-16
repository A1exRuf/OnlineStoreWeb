using Domain.Entities;
using Domain.Exceptions.Base;

namespace Domain.Exceptions
{
    public class NotFoundByIdException<TEntity> : NotFoundException
        where TEntity : Entity
    {
        public NotFoundByIdException(Guid Id) 
            : base($"The {typeof(TEntity).Name} with the identifier {Id} was not found")
        {
        }
    }
}
