namespace GestionaT.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommandRequest(string Name, decimal Price, Guid CategoryId);
}
