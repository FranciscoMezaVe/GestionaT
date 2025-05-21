using GestionaT.Application.Features.Auth.Commands.LoginCommand;
using MediatR;
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

        // POST api/<AuthController>/login
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
    }
}
