using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Roles
{
    public class RolesResponse : BaseEntity
    {
        public required string Name { get; set; }
    }
}
