using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(CreateCategoryCommandRequest Request, Guid BusinessId) : IRequest<Result<Guid>>;
}
