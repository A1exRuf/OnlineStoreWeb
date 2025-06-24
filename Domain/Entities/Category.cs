namespace Domain.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public Category ParentCategory { get; private set; }
        public List<Product> Products { get; private set; }

        public Category(Guid id, string name, Guid? parentId) : base(id)
        {
            Name = name;
            ParentCategoryId = parentId;
            Products = [];
        }

        public Category () { }
    }
}