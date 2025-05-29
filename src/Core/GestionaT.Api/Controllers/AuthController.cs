using GestionaT.Api.Common.Result;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Auth.Commands.LoginCommand;
using GestionaT.Application.Features.Auth.Commands.LogoutCommand;
using GestionaT.Application.Features.Auth.Commands.OAuthLoginCommand;
using GestionaT.Application.Features.Auth.Commands.RefreshTokenCommand;
using GestionaT.Application.Features.Auth.Commands.RegisterCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;

        public AuthController(IMediator mediator, ILogger<AuthController> logger, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _mediator = mediator;
            _logger = logger;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(request);

            return result.ToActionResult(_httpStatusCodeResolver);
        }

        // POST api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(request);

            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommandRequest request)
        {
            var result = await _mediator.Send(new RegisterCommand(request));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpPost("oauth-login")]
        public async Task<IActionResult> OAuthLogin([FromBody] OAuthLoginCommand request)
        {
            var result = await _mediator.Send(request);
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
