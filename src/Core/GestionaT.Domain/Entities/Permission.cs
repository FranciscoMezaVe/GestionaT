using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Permission : BaseEntity, ISoftDeletable
    {
        public required string Name { get; set; } // Ej: "ManageProducts", "ViewSales"
        public bool IsDeleted { get; set; }
    }
}
