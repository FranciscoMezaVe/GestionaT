using GestionaT.Application.Common;
using GestionaT.Application.Features.Roles;
using GestionaT.Application.Features.Roles.Commands.CreateRolesCommand;
using GestionaT.Application.Features.Roles.Commands.DeleteRoleCommand;
using GestionaT.Application.Features.Roles.Commands.UpdateRoleCommand;
using GestionaT.Application.Features.Roles.Queries.GetAllRolesQuery;
using GestionaT.Application.Features.Roles.Queries.GetRoleByIdQuery;
using GestionaT.Infraestructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IMediator _mediator;

        public RolesController(ILogger<RolesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateRoles([FromBody] CreateRolesCommand request, Guid businessId)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            request.BusinessId = businessId;

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al crear el rol.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al crear el rol.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("rol creado: {rol}", result.Value);
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolesResponse>>> GetAllRoles(Guid businessId)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(businessId));
            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al obtener los roles.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al obtener los roles.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            return Ok(result.Value);
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<RolesResponse>> GetRoleById(Guid businessId, Guid roleId)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(businessId, roleId));
            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedió un error al obtener el rol con ID {RoleId}", roleId);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al obtener el rol.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid businessId, Guid id, [FromBody] UpdateRoleCommandRequest request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(request, id, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al actualizar el rol.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al eliminar el rol.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return NoContent();
        }
    }
}
