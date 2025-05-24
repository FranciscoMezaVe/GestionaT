using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(UpdateCategoryCommandRequest Request, Guid Id, Guid BusinessId) : IRequest<Result>;
}