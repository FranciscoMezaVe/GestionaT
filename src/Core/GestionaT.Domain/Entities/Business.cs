using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Business : BaseEntity, ISoftDeletable
    {
        public Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public ICollection<Members> Members { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Sale> Sales { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<Category> Categories { get; set; }
        public bool IsDeleted { get; set; }
    }
}
