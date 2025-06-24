namespace Domain.Entities;

public class Cart : Entity
{
    public Guid? UserId { get; set; }
    public User User { get; set; }
    public List<CartItem> Items { get; private set; }

    public Cart(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
        Items = [];
    }

    public Cart() { }
}