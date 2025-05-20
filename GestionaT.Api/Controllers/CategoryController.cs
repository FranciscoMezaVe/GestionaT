using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Application.Features.Categories.Commands.UpdatePatchCategory;
using GestionaT.Application.Features.Categories.Queries.GetCategoryById;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IMediator _mediator;

        public CategoryController(ILogger<CategoryController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest();
            }

            var guid = await _mediator.Send(request);
            _logger.LogInformation($"Categoria creada: {guid}");
            return Ok(guid);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Guid>> UpdatePatchCategory(Guid id, [FromBody] JsonPatchDocument<UpdatePatchCategory> request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest();
            }


            //Obtener primero
            var category = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (category is null)
            {
                _logger.LogInformation("Categoria no encontrada");
                return NoContent();
            }
            //
            //Aplicar el patch
            //request.ApplyTo(category, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            var afterCategory = new UpdatePatchCategory(category.Id, category.Name, category.Description);

            if (TryValidateModel(category))
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest(ModelState);
            }

            var guid = await _mediator.Send(request);
            _logger.LogInformation($"Categoria actualizada: {guid}");
            return Ok(guid);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Guid>> GetCategoryById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest();
            }

            var category = await _mediator.Send(new GetCategoryByIdQuery(id));

            if (category is null)
            {
                _logger.LogInformation("Categoria no encontrada");
                return NoContent();
            }

            _logger.LogInformation($"Categoria encontrada: {category}");
            return Ok(category);
        }
    }
}
