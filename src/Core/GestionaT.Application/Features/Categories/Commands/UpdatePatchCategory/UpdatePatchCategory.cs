using GestionaT.Domain.Entities;
using MediatR;

namespace GestionaT.Application.Features.Categories.Commands.UpdatePatchCategory
{
    public record UpdatePatchCategory(Guid Id, string Name, string Description) : IRequest<Guid>;
}
