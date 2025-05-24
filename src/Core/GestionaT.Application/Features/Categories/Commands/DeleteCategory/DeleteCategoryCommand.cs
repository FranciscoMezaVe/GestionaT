using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id, Guid BusinessId) : IRequest<Result>;
}
