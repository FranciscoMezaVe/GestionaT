using GestionaT.Application.Features.Products.Commands.CreateProduct;
using GestionaT.Application.Features.Products.Commands.UpdateProduct;
using GestionaT.Application.Features.Products.Queries.GetAllProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionaT.Application.Features.Products.Queries.GetProductById;
using GestionaT.Application.Features.Products.Commands.DeleteProduct;
using Microsoft.AspNetCore.Authorization;
using GestionaT.Infraestructure.Authorization;
using GestionaT.Application.Features.Products.Commands.DeleteProductImage;
using GestionaT.Application.Features.Products.Commands.AddProductImages;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Common.Errors;
using GestionaT.Api.Common.Result;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _mediator = mediator;
            _logger = logger;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductCommandRequest request, [FromRoute] Guid businessId)
        {
            if (request == null)
            {
                _logger.LogWarning("Solicitud inválida para crear producto.");
                return BadRequest("Solicitud inválida.");
            }

            var result = await _mediator.Send(new CreateProductCommand(request, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
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
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid businessId, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllProductsQuery(businessId, paginationFilters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(businessId, id));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpDelete("{productId}/images/{productImageId}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] Guid businessId, [FromRoute] Guid productId, [FromRoute] Guid productImageId)
        {
            var command = new DeleteProductImageCommand(businessId, productId, productImageId);
            var result = await _mediator.Send(command);
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> AddImages([FromRoute] Guid businessId, [FromRoute] Guid productId, [FromForm] List<IFormFile> images)
        {
            var command = new AddProductImagesCommand(businessId, productId, images);
            var result = await _mediator.Send(command);
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
