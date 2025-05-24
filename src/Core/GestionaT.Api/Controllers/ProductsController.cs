using GestionaT.Application.Common;
using GestionaT.Application.Features.Products.Commands.CreateProduct;
using GestionaT.Application.Features.Products.Commands.UpdateProduct;
using GestionaT.Application.Features.Products.Queries.GetAllProducts;
using GestionaT.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionaT.Application.Features.Products.Queries.GetProductById;
using GestionaT.Application.Features.Products.Commands.DeleteProduct;
using Microsoft.AspNetCore.Authorization;
using GestionaT.Infraestructure.Authorization;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateProductCommandRequest request, Guid businessId)
        {
            if (request == null)
            {
                _logger.LogWarning("Solicitud inválida para crear producto.");
                return BadRequest("Solicitud inválida.");
            }

            var result = await _mediator.Send(new CreateProductCommand(request, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al crear el producto.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid businessId, Guid id, [FromBody] UpdateProductCommandRequest request)
        {
            if (request == null)
            {
                _logger.LogInformation("La petición de actualización está vacía.");
                return BadRequest("La solicitud no es válida.");
            }

            var result = await _mediator.Send(new UpdateProductCommand(request, id, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogWarning("Error al actualizar el producto {ProductId}: {Error}", id, httpError.Message);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al actualizar el producto.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            _logger.LogInformation("Producto actualizado exitosamente: {ProductId}", id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll(Guid businessId)
        {
            var result = await _mediator.Send(new GetAllProductsQuery(businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogWarning("Error al obtener productos: {Error}", httpError.Message);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al obtener los productos.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetById(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(businessId, id));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogWarning("Error al obtener el producto con ID {Id}: {Error}", id, httpError.Message);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al obtener el producto.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al eliminar el producto.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return NoContent();
        }
    }
}
