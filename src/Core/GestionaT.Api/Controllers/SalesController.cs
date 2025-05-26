using GestionaT.Application.Common;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Sales.Commands.CreateSales;
using GestionaT.Application.Features.Sales.Queries;
using GestionaT.Application.Features.Sales.Queries.GetAllSales;
using GestionaT.Application.Features.Sales.Queries.GetReportSales;
using GestionaT.Application.Features.Sales.Queries.GetSaleById;
using GestionaT.Infraestructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Controllers
{
    [Route("api/businesses/{businessId}/[controller]")]
    [ApiController]
    [Authorize]
    [AuthorizeBusinessAccess("businessId")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly IMediator _mediator;

        public SalesController(ILogger<SalesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateSale([FromRoute] Guid businessId, [FromBody] CreateSalesCommandRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(new CreateSalesCommand(request, businessId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al crear la venta.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al crear la venta.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("venta creada: {venta}", result.Value);
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<SalesResponse>>> GetAllSales([FromRoute] Guid businessId, [FromQuery] SalesFilters filters, [FromQuery] PaginationFilters paginationFilters)
        {
            if (filters.From.HasValue != filters.To.HasValue)
            {
                return BadRequest("Debe proporcionar ambas fechas 'From' y 'To' o ninguna.");
            }

            var result = await _mediator.Send(new GetAllSalesQuery(businessId, filters, paginationFilters));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al consultar las ventas.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al consultar las ventas.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("ventas consultadas: {ventas}", result.Value.Items.Count);
            return Ok(result.Value);
        }

        [HttpGet("reports")]
        public async Task<ActionResult<byte[]>> GetSaleReport([FromRoute] Guid businessId, [FromQuery] SalesFilters filters)
        {
            if (filters.From.HasValue != filters.To.HasValue)
            {
                return BadRequest("Debe proporcionar ambas fechas 'From' y 'To' o ninguna.");
            }

            var result = await _mediator.Send(new GetReportSalesQuery(businessId, filters));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al consultar las ventas.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al consultar las ventas.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("Se genero el reporte correctamente");
            return File(result.Value, "application/pdf", "SaleReport.pdf");
        }

        [HttpGet("{saleId:guid}")]
        public async Task<ActionResult<IEnumerable<SalesResponse>>> GetAllSales([FromRoute] Guid businessId, [FromRoute] Guid saleId)
        {

            var result = await _mediator.Send(new GetSaleByIdQuery(businessId, saleId));

            if (!result.IsSuccess)
            {
                var httpError = result.Errors.OfType<HttpError>().First();
                _logger.LogInformation("Sucedio un error al consultar la venta.");
                return StatusCode(httpError.StatusCode, new
                {
                    Message = "Error al consultar la venta.",
                    Errors = result.Errors.Select(e => new
                    {
                        e.Message,
                        e.Reasons
                    })
                });
            }
            _logger.LogInformation("venta consultada: {ventas}", result.Value);
            return Ok(result.Value);
        }
    }
}
