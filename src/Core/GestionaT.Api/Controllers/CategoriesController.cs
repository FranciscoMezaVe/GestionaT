using GestionaT.Application.Common;
using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Application.Features.Categories.Queries.GetAllCategories;
using GestionaT.Application.Features.Categories.Queries;
using GestionaT.Infraestructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionaT.Application.Features.Categories.Commands.UpdateCategory;
using GestionaT.Application.Features.Categories.Commands.DeleteCategory;
using GestionaT.Application.Features.Categories.Queries.GetCategoryById;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Categories.Commands.UploadCategoryImage;

[Route("api/businesses/{businessId}/[controller]")]
[Authorize]
[AuthorizeBusinessAccess("businessId")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryCommandRequest request, Guid businessId)
    {
        if (request == null)
        {
            _logger.LogWarning("Solicitud inválida para crear categoría.");
            return BadRequest("Solicitud inválida.");
        }

        var result = await _mediator.Send(new CreateCategoryCommand(request, businessId));

        if (!result.IsSuccess)
        {
            var httpError = result.Errors.OfType<HttpError>().First();
            return StatusCode(httpError.StatusCode, new
            {
                Message = "Error al crear la categoría.",
                Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
            });
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CategoryResponse>>> GetAllCategories(Guid businessId, [FromQuery] PaginationFilters filters)
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery(businessId, filters));

        if (!result.IsSuccess)
        {
            return StatusCode(500, new { Message = "Error al obtener las categorías.", result.Errors });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetCategoryById(Guid businessId, Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (!result.IsSuccess)
        {
            return StatusCode(500, new { Message = "Error al obtener las categorías.", result.Errors });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid businessId, Guid id, [FromBody] UpdateCategoryCommandRequest request)
    {
        var result = await _mediator.Send(new UpdateCategoryCommand(request, id, businessId));

        if (!result.IsSuccess)
        {
            var httpError = result.Errors.OfType<HttpError>().First();
            return StatusCode(httpError.StatusCode, new
            {
                Message = "Error al actualizar la categoría.",
                Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid businessId, Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id, businessId));

        if (!result.IsSuccess)
        {
            var httpError = result.Errors.OfType<HttpError>().First();
            return StatusCode(httpError.StatusCode, new
            {
                Message = "Error al eliminar la categoría.",
                Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
            });
        }

        return NoContent();
    }

    [HttpPost("{categoryId}/images")]
    public async Task<IActionResult> UploadCategoryImage([FromForm] UploadCategoryImageCommandRequest request, [FromRoute] Guid categoryId, [FromRoute] Guid businessId)
    {
        var result = await _mediator.Send(new UploadCategoryImageCommand(request.Image, businessId, categoryId));

        if (!result.IsSuccess)
        {
            var httpError = result.Errors.OfType<HttpError>().First();
            _logger.LogInformation("Sucedio un error al subir la imagen.");
            return StatusCode(httpError.StatusCode, new
            {
                Message = "Error al subir la imagen.",
                Errors = result.Errors.Select(e => new
                {
                    e.Message,
                    e.Reasons
                })
            });
        }
        _logger.LogInformation("imagen subida: {url}", result.Value);
        return Ok(result.Value);
    }

}