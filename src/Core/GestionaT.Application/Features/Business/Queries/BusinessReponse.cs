using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Business.Queries
{
    public class BusinessReponse : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public required string Name { get; set; }
    }
}
