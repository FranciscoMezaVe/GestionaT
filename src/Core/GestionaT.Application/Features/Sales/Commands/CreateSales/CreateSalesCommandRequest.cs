namespace GestionaT.Application.Features.Sales.Commands.CreateSales
{
    public record CreateSalesCommandRequest(Guid CustomerId, IEnumerable<ProductRequestDto> Products);

    public record ProductRequestDto(Guid Id, int Quantity);
}
