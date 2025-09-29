using Domain.Exceptions;

namespace Domain.Entities;

public class CartItem : Entity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; private set; }
    public Guid ProductId { get; set; }
    public Product Product { get; private set; }
    public int Quantity { get; set; }

    public CartItem(Guid cartId, Guid productId, int? quantity)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity ?? 1;
    }

    public CartItem() { }
}