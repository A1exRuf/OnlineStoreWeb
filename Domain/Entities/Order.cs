using Domain.Common;

namespace Domain.Entities;

public class Order : Entity
{
    public Guid UserId { get; set; }
    public User User { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public DateTime? ShippingDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public string Address {  get; set; } 
    public List<OrderItem> Items { get; private set; }

    public Order(Guid userId, string address)
    {
        UserId = userId;
        OrderDate = DateTime.UtcNow;
        Address = address;
        Status = OrderStatus.Created;
        Items = [];
    }

    public Order() { }

    public void StatusToPaid()
    {
        Status = OrderStatus.Paid;
        PaidDate = DateTime.UtcNow;
    }

    public void StatusToShiping()
    {
        Status = OrderStatus.Shipping;
        ShippingDate = DateTime.UtcNow;
    }

    public void StatusToComplited()
    {
        Status = OrderStatus.Completed;
        CompletedDate = DateTime.UtcNow;
    }
}