namespace Domain.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Order Order { get; private set; }
    public Guid ProductId { get; set; }
    public Product Product { get; private set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public OrderItem(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice) : base(id)
    { 
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public OrderItem() { }
}
