using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
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
    }
}
