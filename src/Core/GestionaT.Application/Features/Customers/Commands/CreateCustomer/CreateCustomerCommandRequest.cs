namespace GestionaT.Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommandRequest(string Name)
    {
        public string? Rfc { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }
    }
}
