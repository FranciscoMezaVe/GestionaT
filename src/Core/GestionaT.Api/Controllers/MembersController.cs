using GestionaT.Application.Common;
using GestionaT.Application.Features.Members;
using GestionaT.Application.Features.Members.Commands.DeleteMemberCommand;
using GestionaT.Application.Features.Members.Commands.UpdateMemberRoleCommand;
using GestionaT.Application.Features.Members.Queries.GetAllMembersByBusiness;
using GestionaT.Application.Features.Members.Queries.GetMemberById;
using GestionaT.Infraestructure.Authorization;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ILogger<MembersController> _logger;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public MembersController(ILogger<MembersController> logger, IMediator mediator, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> DeleteMember(Guid memberId, Guid businessId)
        {
            var result = await _mediator.Send(new DeleteMemberCommand(memberId, businessId));

            if (!result.IsSuccess)
            {
                var error = result.Errors.OfType<HttpError>().FirstOrDefault();
                _logger.LogInformation("Error al desactivar el miembro.");
                return StatusCode(error?.StatusCode ?? 422, new
                {
                    Message = "Error al desactivar el miembro.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Miembro desactivado correctamente.");
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembersResponse>>> GetAllMembersByBusiness(Guid businessId)
        {
            var result = await _mediator.Send(new GetAllMembersByBusinessQuery(businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al obtener los miembros.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            return Ok(result.Value);
        }

        [HttpGet("{memberId}")]
        public async Task<ActionResult<MembersResponse>> GetMemberById(Guid businessId, Guid memberId)
        {
            var result = await _mediator.Send(new GetMemberByIdQuery(businessId, memberId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al consultar el miembro.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            return Ok(result.Value);
        }

        [HttpPatch("{memberId}/role")]
        public async Task<IActionResult> UpdateMemberRole(Guid businessId, Guid memberId, [FromBody] UpdateMemberRoleDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Solicitud inválida para actualizar rol.");
                return BadRequest("La solicitud es inválida.");
            }

            var result = await _mediator.Send(new UpdateMemberRoleCommand(businessId, memberId, request));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al actualizar el rol del miembro.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            _logger.LogInformation("Rol del miembro {MemberId} actualizado exitosamente.", memberId);
            return NoContent();
        }
    }
}
