using GestionaT.Domain.Abstractions;
using GestionaT.Domain.Enums;

namespace GestionaT.Domain.Entities
{
    public class Members : BaseEntity
    {
        public Guid UserId { get; set; } // FK a ApplicationUser.Id
        public Guid BusinessId { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsAccepted { get; set; } = false;
        public Status Active { get; set; } = Status.Active;
    }
}
