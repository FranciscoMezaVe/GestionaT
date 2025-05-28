using GestionaT.Application.Common;
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

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody]LoginCommand request)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                _logger.LogInformation("Sucedio un error al iniciar sesion.");
                return Unauthorized(new
                {
                    Message = "Error al iniciar sesion.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("Sesion iniciada, token: {@token}", result.Value);
            return Ok(result.Value);
        }

        // POST api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken([FromBody] RefreshTokenCommand request)
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
                _logger.LogInformation("Sucedio un error al refrescar el token.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al refrescar el token",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("token refrescado: {refreshToken}, token: {token}", result.Value.NewRefreshToken, result.Value.NewToken);
            return Ok(result.Value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommandRequest request)
        {
            var result = await _mediator.Send(new RegisterCommand(request));

            if (result.IsFailed)
            {
                // Extraer todos los mensajes de error de Result
                var errors = result.Errors
                    .SelectMany(e => e.Reasons)
                    .Select(r => r.Message)
                    .ToList();

                return BadRequest(new { errors });
            }

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());

            if (result.IsFailed)
            {
                // Extraer todos los mensajes de error de Result
                var errors = result.Errors
                    .SelectMany(e => e.Reasons)
                    .Select(r => r.Message)
                    .ToList();

                return BadRequest(errors);
            }

            return NoContent();
        }

        [HttpPost("oauth-login")]
        public async Task<IActionResult> OAuthLogin([FromBody] OAuthLoginCommand request)
        {
            var result = await _mediator.Send(request);

            if (result.IsFailed)
            {
                // Extraer todos los mensajes de error de Result
                var errors = result.Errors
                    .SelectMany(e => e.Reasons)
                    .Select(r => r.Message)
                    .ToList();

                return BadRequest(errors);
            }

            return Ok(result.Value);
        }
    }
}
