namespace Domain.Entities;

public class ResetToken : Entity
{
    public string Token { get; set; }
    public DateTime ExpiresOnUtc { get; private set; }

    public Guid UserId { get; set; }
    public User User { get; private set; }

    public ResetToken(Guid id, Guid userId, string token) : base(id)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresOnUtc = DateTime.UtcNow.AddMinutes(30);
    }

    public ResetToken() { }
}
