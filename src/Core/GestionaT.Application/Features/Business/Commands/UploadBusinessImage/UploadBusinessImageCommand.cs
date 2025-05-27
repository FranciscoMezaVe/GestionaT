using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Features.Business.Commands.UploadBusinessImage
{
    public record UploadBusinessImageCommand(Guid BusinessId, IFormFile Image) : IRequest<Result<string>>;
}
