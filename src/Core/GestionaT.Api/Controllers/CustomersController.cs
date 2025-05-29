using GestionaT.Application.Features.Customers.Commands.CreateCustomer;
using GestionaT.Application.Features.Customers.Commands.UpdateCustomer;
using GestionaT.Application.Features.Customers.Commands.DeleteCustomer;
using GestionaT.Application.Features.Customers.Queries.GetAllCustomers;
using GestionaT.Application.Features.Customers.Queries.GetCustomerById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionaT.Infraestructure.Authorization;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Common.Errors;
using GestionaT.Api.Common.Result;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(IMediator mediator, ILogger<CustomersController> logger, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _mediator = mediator;
            _logger = logger;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommandRequest request, Guid businessId)
        {
            if (request == null)
            {
                _logger.LogWarning("Solicitud inválida para crear cliente.");
                return BadRequest("Solicitud inválida.");
            }

            var result = await _mediator.Send(new CreateCustomerCommand(request, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
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
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid businessId, [FromQuery] PaginationFilters filters)
        {
            var result = await _mediator.Send(new GetAllCustomersQuery(businessId, filters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(businessId, id));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid id)
        {
            var result = await _mediator.Send(new DeleteCustomerCommand(id, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
