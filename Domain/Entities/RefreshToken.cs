namespace Domain.Entities;

public class RefreshToken : Entity
{
    public string Token { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public RefreshToken(string token, Guid userId)
    {
        Token = token;
        ExpiresOnUtc = DateTime.UtcNow.AddDays(7);
        UserId = userId;
    }

    public RefreshToken() { }

    public void UpdateToken(string token)
    {
        Token = token;
        ExpiresOnUtc = DateTime.UtcNow.AddDays(7);
    }
}
