using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionaT.Application.Features.Customers.Commands.UpdateCustomer
{
    public record UpdateCustomerCommandRequest
    {
        public required string Name { get; set; }
        public string? Rfc { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }
    }
}
