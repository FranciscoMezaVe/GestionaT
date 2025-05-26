using GestionaT.Application.Common;
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

        public UsersController(ILogger<UsersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
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

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al subir la imagen.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al subir la imagen.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("imagen subida: {url}", result.Value);
            return Ok(result.Value);
        }

        [HttpGet("images")]
        [Authorize]
        public async Task<IActionResult> GetProfileImage()
        {
            var result = await _mediator.Send(new GetUserImageQuery());

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al consultar la imagen.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al consultar la imagen.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }

            if (string.IsNullOrWhiteSpace(result.Value))
            {
                return NoContent();
            }

            _logger.LogInformation("imagen consultada: {url}", result.Value);
            return Ok(result.Value);
        }
    }
}
