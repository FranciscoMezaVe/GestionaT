using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, string Description) 
        : IRequest<Result<Guid>>;
}
