using Domain.Common;

namespace Domain.Entities;

public class User : Entity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; private set; }
    public List<Order> Orders { get; private set; }

    public User (Guid id, string email, string passwordHash, UserRole role) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        Orders = [];
    }

    private User () { }
}
