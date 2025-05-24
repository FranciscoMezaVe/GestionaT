using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Categories.Queries
{
    public class CategoryResponse : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
