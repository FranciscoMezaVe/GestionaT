using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Products.Queries
{
    public class ProductResponse : BaseEntity
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
    }
}
