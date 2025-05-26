using GestionaT.Application.Common;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Invitations.Commands.AcceptInvitationCommand;
using GestionaT.Application.Features.Invitations.Commands.CreateInvitation;
using GestionaT.Application.Features.Invitations.Commands.CreateInvitationCommand;
using GestionaT.Application.Features.Invitations.Commands.RejectInvitationCommand;
using GestionaT.Application.Features.Invitations.Queries.GetAllInivitationsByUser;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using GestionaT.Domain.Entities;
using GestionaT.Infraestructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class InvitationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InvitationsController> _logger;

        public InvitationsController(IMediator mediator, ILogger<InvitationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Envía una invitación a un usuario para unirse a un negocio.
        /// Solo el Owner puede hacerlo.
        /// </summary>
        /// 
        [AuthorizeBusinessAccess("businessId")]
        [HttpPost("businesses/{businessId}/[controller]")]
        public async Task<ActionResult<Guid>> CreateInvitation([FromBody] CreateInvitationCommandDto request, Guid businessId)
        {
            var result = await _mediator.Send(new CreateInvitationCommand(businessId, request));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();

                _logger.LogWarning("Error al crear invitación: {error}", httpError?.Message ?? "Desconocido");
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al crear invitación.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Invitación creada correctamente: {invitationId}", result.Value);
            return Ok(result.Value);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpGet("businesses/{businessId}/[controller]")]
        public async Task<ActionResult<IEnumerable<InvitationResponse>>> GetAllInvitation(Guid businessId, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllInvitationsQuery(businessId, paginationFilters));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();

                _logger.LogWarning("Error al consultar las invitaciones: {error}", httpError?.Message ?? "Desconocido");
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al consultar las invitaciones.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Invitaciones consultadas correctamente, se econtraron: {invitationId}", result.Value.Items.Count);
            return Ok(result.Value);
        }

        [HttpPost("[controller]/{invitationId}/accept")]
        public async Task<ActionResult> Accept(Guid invitationId)
        {
            var result = await _mediator.Send(new AcceptInvitationCommand(invitationId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();

                _logger.LogWarning("Error al aceptar la invitacion: {error}", httpError?.Message ?? "Desconocido");
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al aceptar la invitacion.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Invitacion aceptada");
            return NoContent();
        }

        [HttpPost("[controller]/{invitationId}/reject")]
        public async Task<ActionResult> Reject(Guid invitationId)
        {
            var result = await _mediator.Send(new RejectInvitationCommand(invitationId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();

                _logger.LogWarning("Error al rechazar la invitacion: {error}", httpError?.Message ?? "Desconocido");
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al rechazar la invitacion.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Invitacion rechazada");
            return NoContent();
        }

        [HttpGet("[controller]")]
        public async Task<ActionResult<IEnumerable<InvitationResponse>>> GetAllInvitationByUser([FromQuery] GetAllInvitationByUserQueryFilters filters, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllInvitationByUserQuery(filters, paginationFilters));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().FirstOrDefault();

                _logger.LogWarning("Error al consultar las invitaciones: {error}", httpError?.Message ?? "Desconocido");
                return StatusCode(httpError?.StatusCode ?? 422, new
                {
                    Message = "Error al consultar las invitaciones.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            _logger.LogInformation("Invitaciones consultadas correctamente, se econtraron: {invitationId}", result.Value.Items.Count);
            return Ok(result.Value);
        }
    }
}