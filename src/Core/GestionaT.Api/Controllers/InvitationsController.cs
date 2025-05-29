using GestionaT.Api.Common.Result;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Invitations.Commands.AcceptInvitationCommand;
using GestionaT.Application.Features.Invitations.Commands.CreateInvitation;
using GestionaT.Application.Features.Invitations.Commands.CreateInvitationCommand;
using GestionaT.Application.Features.Invitations.Commands.RejectInvitationCommand;
using GestionaT.Application.Features.Invitations.Queries.GetAllInivitationsByUser;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
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
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;
        private readonly ILogger<InvitationsController> _logger;

        public InvitationsController(IMediator mediator, ILogger<InvitationsController> logger, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _mediator = mediator;
            _logger = logger;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        /// <summary>
        /// Envía una invitación a un usuario para unirse a un negocio.
        /// Solo el Owner puede hacerlo.
        /// </summary>
        /// 
        [AuthorizeBusinessAccess("businessId")]
        [HttpPost("businesses/{businessId}/[controller]")]
        public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationCommandDto request, Guid businessId)
        {
            var result = await _mediator.Send(new CreateInvitationCommand(businessId, request));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [AuthorizeBusinessAccess("businessId")]
        [HttpGet("businesses/{businessId}/[controller]")]
        public async Task<IActionResult> GetAllInvitation(Guid businessId, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllInvitationsQuery(businessId, paginationFilters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpPost("[controller]/{invitationId}/accept")]
        public async Task<IActionResult> Accept(Guid invitationId)
        {
            var result = await _mediator.Send(new AcceptInvitationCommand(invitationId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpPost("[controller]/{invitationId}/reject")]
        public async Task<IActionResult> Reject(Guid invitationId)
        {
            var result = await _mediator.Send(new RejectInvitationCommand(invitationId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("[controller]")]
        public async Task<IActionResult> GetAllInvitationByUser([FromQuery] GetAllInvitationByUserQueryFilters filters, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllInvitationByUserQuery(filters, paginationFilters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}