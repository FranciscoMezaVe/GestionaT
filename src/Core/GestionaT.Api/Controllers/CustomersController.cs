using GestionaT.Application.Common;
using GestionaT.Application.Features.Customers.Commands.CreateCustomer;
using GestionaT.Application.Features.Customers.Commands.UpdateCustomer;
using GestionaT.Application.Features.Customers.Commands.DeleteCustomer;
using GestionaT.Application.Features.Customers.Queries.GetAllCustomers;
using GestionaT.Application.Features.Customers.Queries.GetCustomerById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionaT.Infraestructure.Authorization;
using GestionaT.Application.Features.Customers;
using GestionaT.Application.Common.Pagination;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateCustomerCommandRequest request, Guid businessId)
        {
            if (request == null)
            {
                _logger.LogWarning("Solicitud inválida para crear cliente.");
                return BadRequest("Solicitud inválida.");
            }

            var result = await _mediator.Send(new CreateCustomerCommand(request, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al crear el cliente.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid businessId, Guid id, [FromBody] UpdateCustomerCommandRequest request)
        {
            if (request == null)
            {
                _logger.LogInformation("La petición de actualización está vacía.");
                return BadRequest("La solicitud no es válida.");
            }

            var result = await _mediator.Send(new UpdateCustomerCommand(request, id, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogWarning("Error al actualizar el cliente {CustomerId}: {Error}", id, httpError.Message);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al actualizar el cliente.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            _logger.LogInformation("Cliente actualizado exitosamente: {CustomerId}", id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll(Guid businessId, [FromQuery] PaginationFilters filters)
        {
            var result = await _mediator.Send(new GetAllCustomersQuery(businessId, filters));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogWarning("Error al obtener clientes: {Error}", httpError.Message);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al obtener los clientes.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetById(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(businessId, id));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogWarning("Error al obtener el cliente con ID {Id}: {Error}", id, httpError.Message);
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al obtener el cliente.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new DeleteCustomerCommand(id, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al eliminar el cliente.",
                    Errors = result.Errors.Select(e => new { e.Message, e.Reasons })
                });
            }

            return NoContent();
        }
    }
}
