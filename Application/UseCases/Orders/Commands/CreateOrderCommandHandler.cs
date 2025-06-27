using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentEmail.Core;

namespace Application.UseCases.Orders.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFluentEmail _fluetEmail;

    public CreateOrderCommandHandler(
        IRepository<Order> orderRepository, 
        IRepository<Cart> cartRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService,
        IFluentEmail fluentEmail)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fluetEmail = fluentEmail;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UserNotAuthenticatedException();

        var cart = await _cartRepository.GetAsync(
            filter: new CartFilter { UserId = userId },
            asNoTracking: false,
            cancellationToken,
            includes: new[] { "Items.Product" });

        var order = new Order(userId, request.Address);

        foreach (var item in cart!.Items) 
        { 
            if(item.Product.StockQuantity < item.Quantity)
                throw new NotEnoughProductInStockException(item.Product.Name);

            order.Items.Add(new OrderItem(
                order.Id,
                item.ProductId,
                item.Quantity,
                item.Product.Price));

            item.Product.ApplyPurchase(item.Quantity);
        }

        cart.Items.Clear();
        _cartRepository.Update(cart);

        await _orderRepository.AddAsync(order, cancellationToken);

        var userEmail = _currentUserService.Email
            ?? throw new UserNotAuthenticatedException();

        await _fluetEmail
            .To(userEmail)
            .Subject("New order")
            .Body("Your order has been successfully placed")
            .SendAsync(cancellationToken);

        await _unitOfWork.SaveChangesAsync();

        return order.Id;
    }
}   