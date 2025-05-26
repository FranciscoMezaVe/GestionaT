namespace GestionaT.Application.Features.Sales.Queries
{
    public class SalesFilters
    {
        public Guid? Customer { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
