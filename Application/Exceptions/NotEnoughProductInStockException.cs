using Domain.Entities;
using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class NotEnoughProductInStockException(string name) 
    : BadRequestException($"Not enough {name} in stock")
{
}
