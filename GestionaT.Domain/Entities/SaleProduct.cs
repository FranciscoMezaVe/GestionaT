namespace GestionaT.Domain.Entities
{
    public class SaleProduct
    {
        public Guid SaleId { get; set; }
        public required Sale Sale { get; set; }

        public Guid ProductId { get; set; }
        public required Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
