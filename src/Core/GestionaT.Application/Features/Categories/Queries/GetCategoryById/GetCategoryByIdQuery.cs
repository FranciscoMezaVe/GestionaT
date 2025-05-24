using FluentResults;
using GestionaT.Domain.Entities;
using MediatR;

namespace GestionaT.Application.Features.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id)
        : IRequest<Result<CategoryResponse>>;
}
