using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Roles
{
    public class RolesResponse : BaseEntity
    {
        public Guid BusinessId { get; set; }
        public required string Name { get; set; }
    }
}
