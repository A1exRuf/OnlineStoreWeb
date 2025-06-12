namespace Domain.Entities;

public class Product : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public List<ProductImage> Images { get; set; }

    public Product(Guid id, string name, string description, decimal price, int stockQuantity, Guid categoryId) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        Images = [];
    }

    private Product() { }
}
