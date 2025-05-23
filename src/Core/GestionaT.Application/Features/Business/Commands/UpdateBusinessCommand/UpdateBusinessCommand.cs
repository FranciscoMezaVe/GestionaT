using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Business.Commands.UpdateBusinessCommand
{
    public record UpdateBusinessCommand(Guid BusinessId, UpdateBusinessDto Business) : IRequest<Result>;
}