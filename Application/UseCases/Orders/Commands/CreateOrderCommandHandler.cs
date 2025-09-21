using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Orders.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntityLoader _entityLoader;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public CreateOrderCommandHandler(
        IRepository<Order> orderRepository, 
        IRepository<Cart> cartRepository, 
        IUnitOfWork unitOfWork, 
        IEntityLoader entityLoader,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _entityLoader = entityLoader;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UserNotAuthenticatedException();

        Cart cart = await GetCartWithItemsById(userId, cancellationToken);

        Order order = await CreateOrder(request, userId, cart, cancellationToken);

        ClearCart(cart);

        await _unitOfWork.SaveChangesAsync();

        await SendEmail(cancellationToken);

        return order.Id;
    }

    private async Task<Cart> GetCartWithItemsById(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetAsync(
            filter: new CartFilter { UserId = userId },
            cancellationToken,
            includes: c => c.Items)
            ?? throw new NotFoundByIdException<Cart>(userId);

        await _entityLoader.LoadAsync(
            cart.Items,
            i => i.Product,
            cancellationToken);

        return cart;
    }

    private async Task<Order> CreateOrder(CreateOrderCommand request, Guid userId, Cart cart, CancellationToken cancellationToken)
    {
        var order = new Order(userId, request.Address);

        foreach (var item in cart.Items)
        {
            if (item.Product.StockQuantity < item.Quantity)
                throw new NotEnoughProductInStockException(item.Product.Name);

            order.Items.Add(new OrderItem(
                order.Id,
                item.ProductId,
                item.Quantity,
                item.Product.Price));

            item.Product.ApplyPurchase(item.Quantity);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        return order;
    }

    private void ClearCart(Cart cart)
    {
        cart.Items.Clear();
        _cartRepository.Update(cart);
    }

    private async Task SendEmail(CancellationToken cancellationToken)
    {
        var userEmail = _currentUserService.Email
            ?? throw new UserNotAuthenticatedException();

        await _emailService.SendEmailAsync(
            userEmail,
            "New order",
            "Your order has been succesfully placed",
            cancellationToken);
    }
}   