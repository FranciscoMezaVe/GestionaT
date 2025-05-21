using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Auth.Commands.LoginCommand
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;
}
