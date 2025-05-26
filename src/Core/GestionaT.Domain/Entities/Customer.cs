using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Customer : BaseEntity, ISoftDeletable
    {
        public required string Name { get; set; }
        public string? Rfc { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }
        public Guid BusinessId { get; set; }
        public Business Business { get; set; }
        public ICollection<Sale> Sales { get; set; }
        public bool IsDeleted { get; set; }
    }
}
