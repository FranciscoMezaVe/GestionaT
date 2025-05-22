using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Business.Commands.CreateBusinessCommand
{
    public record CreateBusinessCommand(string Name) : IRequest<Result<Guid>>;
}
