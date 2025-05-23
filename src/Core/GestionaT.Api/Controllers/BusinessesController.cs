using GestionaT.Application.Common;
using GestionaT.Application.Features.Business.Commands.CreateBusinessCommand;
using GestionaT.Application.Features.Business.Commands.DeleteBusinessCommand;
using GestionaT.Application.Features.Business.Commands.UpdateBusinessCommand;
using GestionaT.Application.Features.Business.Queries;
using GestionaT.Application.Features.Business.Queries.GetAllBusinessesQuery;
using GestionaT.Application.Features.Business.Queries.GetBusinessByIdQuery;
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
        private readonly ICurrentUserService _currentUserService;

        public BusinessesController(ILogger<BusinessesController> logger, IMediator mediator, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateBusiness([FromBody] CreateBusinessCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al crear el negocio.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al crear el negocio",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("Negocio creado: {business}", result.Value);
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessReponse>>> GetAllBusinesses()
        {
            Guid userId = _currentUserService.UserId!.Value;
            var result = await _mediator.Send(new GetAllBusinessesQuery(userId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al consultar sus negocios.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al consultar la categoria",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("{businessesCount} negocios encontrados", result.Value.Count());
            return Ok(result.Value);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpGet("{businessId}")]
        public async Task<ActionResult<IEnumerable<BusinessReponse>>> GetBusinessById(Guid businessId)
        {
            var result = await _mediator.Send(new GetBusinessByIdQuery(businessId));

            if (!result.IsSuccess)
            {
                _logger.LogInformation("Sucedio un error al consultar el negocio.");
                return UnprocessableEntity(new
                {
                    Message = "Error al consultar el negocio.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Negocio encontrado {businessesCount}", result.Value);
            return Ok(result.Value);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpDelete("{businessId}")]
        public async Task<IActionResult> DeleteBusiness(Guid businessId)
        {
            var result = await _mediator.Send(new DeleteBusinessCommand(businessId));

            if (result.IsFailed)
            {
                _logger.LogWarning("Error al eliminar el negocio");
                return NotFound(new
                {
                    Message = "No se encontró el negocio para eliminar.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Negocio eliminado: {businessId}", businessId);
            return NoContent();
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpPut("{businessId}")]
        public async Task<ActionResult> UpdateBusiness(Guid businessId, [FromBody] UpdateBusinessDto request)
        {
            if (request == null)
            {
                _logger.LogInformation("La solicitud de actualización no es válida.");
                return BadRequest("Solicitud inválida.");
            }

            var result = await _mediator.Send(new UpdateBusinessCommand(businessId, request));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();
                _logger.LogInformation("Error al actualizar el negocio.");
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al actualizar el negocio.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Negocio actualizado correctamente.");
            return NoContent();
        }
    }
}
