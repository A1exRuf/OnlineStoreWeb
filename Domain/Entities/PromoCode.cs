namespace Domain.Entities;

public class PromoCode : Entity
{
    public  string Code { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }

    public PromoCode(Guid id, string code, decimal discountValue, DateTime expiryDate, bool isActive) : base(id)
    {
        Code = code;
        DiscountValue = discountValue;
        ExpiryDate = expiryDate;
        IsActive = isActive;
    }

    private PromoCode() { }
}
