using Application.Abstractions.Messaging;
using Application.Dtos.Cart;

namespace Application.UseCases.Carts.Queries.Get;

public record GetCartQuery() : IQuery<CartDto>;
