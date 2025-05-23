using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Business.Commands.DeleteBusinessCommand
{
    public record DeleteBusinessCommand(Guid BusinessId) : IRequest<Result>;
}