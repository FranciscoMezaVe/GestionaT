using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand
        : IRequest<Result<Guid>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid BusinessId { get; set; }
    }
}
