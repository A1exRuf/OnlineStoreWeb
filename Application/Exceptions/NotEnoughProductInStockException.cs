using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class NotEnoughProductInStockException() 
    : BadRequestException($"You cannot take more items than are in stock")
{
}
