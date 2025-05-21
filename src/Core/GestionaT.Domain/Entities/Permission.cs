using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public required string Name { get; set; } // Ej: "ManageProducts", "ViewSales"
    }
}
