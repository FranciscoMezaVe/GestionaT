using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Users.Commands.UploadUserImage
{
    public record UploadUserImageCommand(IFormFile Image) : IRequest<Result<string>>;
}
