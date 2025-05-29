using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Application.Features.Categories.Queries.GetAllCategories;
using GestionaT.Infraestructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionaT.Application.Features.Categories.Commands.UpdateCategory;
using GestionaT.Application.Features.Categories.Commands.DeleteCategory;
using GestionaT.Application.Features.Categories.Queries.GetCategoryById;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Categories.Commands.UploadCategoryImage;
using GestionaT.Application.Common.Errors;
using GestionaT.Api.Common.Result;

[Route("api/businesses/{businessId}/[controller]")]
[Authorize]
[AuthorizeBusinessAccess("businessId")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly HttpStatusCodeResolver _httpStatusCodeResolver;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger, HttpStatusCodeResolver httpStatusCodeResolver)
    {
        _mediator = mediator;
        _logger = logger;
        _httpStatusCodeResolver = httpStatusCodeResolver;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommandRequest request, Guid businessId)
    {
        if (request == null)
        {
            _logger.LogWarning("Solicitud inválida para crear categoría.");
            return BadRequest("Solicitud inválida.");
        }

        var result = await _mediator.Send(new CreateCategoryCommand(request, businessId));
        return result.ToActionResult(_httpStatusCodeResolver);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories(Guid businessId, [FromQuery] PaginationFilters filters)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(businessId, filters));
        return result.ToActionResult(_httpStatusCodeResolver);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid businessId, Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return result.ToActionResult(_httpStatusCodeResolver);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid businessId, Guid id, [FromBody] UpdateCategoryCommandRequest request)
    {
        var result = await _mediator.Send(new UpdateCategoryCommand(request, id, businessId));
        return result.ToActionResult(_httpStatusCodeResolver);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid businessId, Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id, businessId));
        return result.ToActionResult(_httpStatusCodeResolver);
    }

    [HttpPost("{categoryId}/images")]
    public async Task<IActionResult> UploadCategoryImage([FromForm] UploadCategoryImageCommandRequest request, [FromRoute] Guid categoryId, [FromRoute] Guid businessId)
    {
        var result = await _mediator.Send(new UploadCategoryImageCommand(request.Image, businessId, categoryId));
        return result.ToActionResult(_httpStatusCodeResolver);
    }

}