namespace GestionaT.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}