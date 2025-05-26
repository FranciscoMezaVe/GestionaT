using GestionaT.Application.Features.Sales.Commands.CreateSales;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public static class SalesMappingConfigurationExtensions
    {
        public static Sale MapToSaleEntity(this CreateSalesCommand command)
        {
            return new Sale
            {
                Date = DateTime.UtcNow,
                BusinessId = command.BusinessId,
                CustomerId = command.Request.CustomerId,
                SaleProducts = command.Request.Products.Select(p => new SaleProduct
                {
                    ProductId = p.Id,
                    Quantity = p.Quantity
                }).ToList()
            };
        }
    }
}
