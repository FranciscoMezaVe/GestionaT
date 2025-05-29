using GestionaT.Api.Common.Result;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
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
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;

        public RolesController(ILogger<RolesController> logger, IMediator mediator, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _logger = logger;
            _mediator = mediator;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoles([FromBody] CreateRolesCommand request, Guid businessId)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            request.BusinessId = businessId;

            var result = await _mediator.Send(request);
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles(Guid businessId, [FromQuery] PaginationFilters paginationFilters)
        {
            var result = await _mediator.Send(new GetAllRolesQuery(businessId, paginationFilters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(Guid businessId, Guid roleId)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(businessId, roleId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid businessId, Guid id, [FromBody] UpdateRoleCommandRequest request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(request, id, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
