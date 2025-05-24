using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInvitations
{
    public class InvitationResponse : BaseEntity
    {
        public Guid BusinessId { get; set; }
        public required string BusinessName { get; set; }
    }
}