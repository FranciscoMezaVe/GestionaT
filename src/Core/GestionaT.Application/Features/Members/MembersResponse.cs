using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Members
{
    public class MembersResponse : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
