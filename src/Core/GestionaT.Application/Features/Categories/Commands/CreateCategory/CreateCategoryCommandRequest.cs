namespace GestionaT.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
