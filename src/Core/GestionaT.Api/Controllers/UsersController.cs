using GestionaT.Api.Common.Result;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Users.Queries.GetUserImage;
using GestionaT.Application.Users.Commands.UploadUserImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;

        public UsersController(ILogger<UsersController> logger, IMediator mediator, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _logger = logger;
            _mediator = mediator;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpPost("images")]
        [Authorize]
        public async Task<IActionResult> UploadProfileImage([FromForm] UploadUserImageCommand command)
        {
            if (command == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(command);
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("images")]
        [Authorize]
        public async Task<IActionResult> GetProfileImage()
        {
            var result = await _mediator.Send(new GetUserImageQuery());
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
