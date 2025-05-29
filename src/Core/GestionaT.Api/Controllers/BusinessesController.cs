using GestionaT.Api.Common.Result;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Business.Commands.CreateBusinessCommand;
using GestionaT.Application.Features.Business.Commands.DeleteBusinessCommand;
using GestionaT.Application.Features.Business.Commands.UpdateBusinessCommand;
using GestionaT.Application.Features.Business.Commands.UploadBusinessImage;
using GestionaT.Application.Features.Business.Queries.GetAllBusinessesQuery;
using GestionaT.Application.Features.Business.Queries.GetBusinessByIdQuery;
using GestionaT.Application.Features.Categories.Commands.UploadCategoryImage;
using GestionaT.Infraestructure.Authorization;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessesController : ControllerBase
    {
        private readonly ILogger<BusinessesController> _logger;
        private readonly IMediator _mediator;
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;
        private readonly ICurrentUserService _currentUserService;

        public BusinessesController(ILogger<BusinessesController> logger, IMediator mediator, ICurrentUserService currentUserService, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _logger = logger;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness([FromBody] CreateBusinessCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(request);
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinesses([FromQuery] PaginationFilters filters)
        {
            Guid userId = _currentUserService.UserId!.Value;
            var result = await _mediator.Send(new GetAllBusinessesQuery(userId, filters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetBusinessById(Guid businessId)
        {
            var result = await _mediator.Send(new GetBusinessByIdQuery(businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpDelete("{businessId}")]
        public async Task<IActionResult> DeleteBusiness(Guid businessId)
        {
            var result = await _mediator.Send(new DeleteBusinessCommand(businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpPut("{businessId}")]
        public async Task<IActionResult> UpdateBusiness(Guid businessId, [FromBody] UpdateBusinessDto request)
        {
            if (request == null)
            {
                _logger.LogInformation("La solicitud de actualización no es válida.");
                return BadRequest("Solicitud inválida.");
            }

            var result = await _mediator.Send(new UpdateBusinessCommand(businessId, request));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpPost("{businessId}/images")]
        public async Task<IActionResult> UploadCategoryImage([FromForm] UploadCategoryImageCommandRequest request, [FromRoute] Guid businessId)
        {
            var result = await _mediator.Send(new UploadBusinessImageCommand(businessId, request.Image));
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
