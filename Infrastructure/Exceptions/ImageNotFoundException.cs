using Domain.Exceptions.Base;

namespace Infrastructure.Exceptions;

public class ImageNotFoundException : NotFoundException
{
    public ImageNotFoundException(Guid id)
        : base($"Image with Id {id} not found.")
    {
    }
}
