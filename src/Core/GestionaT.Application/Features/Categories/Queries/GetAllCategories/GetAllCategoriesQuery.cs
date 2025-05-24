using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery(Guid BusinessId) : IRequest<Result<List<CategoryResponse>>>;
}