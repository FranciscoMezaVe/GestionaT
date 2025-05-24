namespace GestionaT.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandRequest
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
    }
}
