using GestionaT.Api.Common.Result;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
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
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;
        private readonly ICurrentUserService _currentUserService;

        public MembersController(ILogger<MembersController> logger, IMediator mediator, ICurrentUserService currentUserService, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _logger = logger;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> DeleteMember(Guid memberId, Guid businessId)
        {
            var result = await _mediator.Send(new DeleteMemberCommand(memberId, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembersByBusiness(Guid businessId, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllMembersByBusinessQuery(businessId, paginationFilters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberById(Guid businessId, Guid memberId)
        {
            var result = await _mediator.Send(new GetMemberByIdQuery(businessId, memberId));
            return result.ToActionResult(_httpStatusCodeResolver);
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
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
