namespace Domain.Entities;

public class RefreshToken : Entity
{
    public string Token { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

}
