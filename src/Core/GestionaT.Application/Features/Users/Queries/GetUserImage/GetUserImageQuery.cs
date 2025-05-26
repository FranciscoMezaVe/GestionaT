using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Users.Queries.GetUserImage
{
    public record GetUserImageQuery() : IRequest<Result<string>>;
}
