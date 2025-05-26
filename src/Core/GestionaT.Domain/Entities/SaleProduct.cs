namespace GestionaT.Domain.Entities
{
    public class SaleProduct
    {
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; }

        public required Guid ProductId { get; set; }
        public  Product Product { get; set; }
        public required int Quantity { get; set; }
    }
}
