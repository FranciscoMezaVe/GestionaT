using GestionaT.Domain.Abstractions;
using GestionaT.Domain.Enums;

namespace GestionaT.Domain.Entities
{
    public class Invitation : BaseEntity
    {
        public Guid BusinessId { get; set; }
        public Business Business { get; set; }
        public Guid InvitedUserId { get; set; }
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    }
}
