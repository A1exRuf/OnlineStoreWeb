using Domain.Entities;
using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class CircularDependencyException<TEntity> : BadRequestException 
    where TEntity : Entity
{
    public CircularDependencyException() 
        : base($"{typeof(TEntity).Name} cannot reference to itself")
    {
    }
}
