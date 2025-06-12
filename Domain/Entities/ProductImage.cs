namespace Domain.Entities;

public class ProductImage : Entity
{
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public Product Product { get; set; }

    public ProductImage(Guid id, Guid productId, string imageUrl, string? altText, int? displayOrder) : base(id)
    {
        ProductId = productId;
        ImageUrl = imageUrl;
        AltText = altText;
        DisplayOrder = displayOrder ?? 1;
    }

    private ProductImage() { }


}