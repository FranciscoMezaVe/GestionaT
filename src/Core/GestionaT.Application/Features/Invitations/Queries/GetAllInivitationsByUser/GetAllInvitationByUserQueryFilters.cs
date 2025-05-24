using GestionaT.Domain.Enums;

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInivitationsByUser
{
    public class GetAllInvitationByUserQueryFilters
    {
        public Guid? Business { get; set; }
        public InvitationStatus? Status { get; set; }
    }
}
