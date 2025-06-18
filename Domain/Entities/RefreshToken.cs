namespace Domain.Entities;

public class RefreshToken : Entity
{
    public string Token { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public RefreshToken(Guid id, string token, Guid userId) : base(id)
    {
        Id = id;
        Token = token;
        ExpiresOnUtc = DateTime.UtcNow.AddDays(7);
        UserId = userId;
    }

    private RefreshToken() { }

    public void UpdateToken(string token)
    {
        Token = token;
        ExpiresOnUtc = DateTime.UtcNow.AddDays(7);
    }
}
