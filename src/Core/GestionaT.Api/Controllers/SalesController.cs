using GestionaT.Api.Common.Result;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
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
        private readonly HttpStatusCodeResolver _httpStatusCodeResolver;

        public SalesController(ILogger<SalesController> logger, IMediator mediator, HttpStatusCodeResolver httpStatusCodeResolver)
        {
            _logger = logger;
            _mediator = mediator;
            _httpStatusCodeResolver = httpStatusCodeResolver;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromRoute] Guid businessId, [FromBody] CreateSalesCommandRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogInformation("La peticion no cuenta con el formato correspondiente");
                return BadRequest("La solicitud no es valida.");
            }

            var result = await _mediator.Send(new CreateSalesCommand(request, businessId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSales([FromRoute] Guid businessId, [FromQuery] SalesFilters filters, [FromQuery] PaginationFilters paginationFilters)
        {
            if (filters.From.HasValue != filters.To.HasValue)
            {
                return BadRequest("Debe proporcionar ambas fechas 'From' y 'To' o ninguna.");
            }

            var result = await _mediator.Send(new GetAllSalesQuery(businessId, filters, paginationFilters));
            return result.ToActionResult(_httpStatusCodeResolver);
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetSaleReport([FromRoute] Guid businessId, [FromQuery] SalesFilters filters)
        {
            if (filters.From.HasValue != filters.To.HasValue)
            {
                return BadRequest("Debe proporcionar ambas fechas 'From' y 'To' o ninguna.");
            }

            var result = await _mediator.Send(new GetReportSalesQuery(businessId, filters));
            return result.ToFileResult("application/pdf", "SaleReport.pdf", _httpStatusCodeResolver, _logger);
        }

        [HttpGet("{saleId:guid}")]
        public async Task<IActionResult> GetAllSales([FromRoute] Guid businessId, [FromRoute] Guid saleId)
        {

            var result = await _mediator.Send(new GetSaleByIdQuery(businessId, saleId));
            return result.ToActionResult(_httpStatusCodeResolver);
        }
    }
}
