using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Business : BaseEntity
    {
        public required string Name { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
