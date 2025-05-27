using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Queries.GetBusinessByIdQuery
{
    public class GetBusinessByIdQueryHandler : IRequestHandler<GetBusinessByIdQuery, Result<BusinessReponse>>
    {
        private readonly ILogger<GetBusinessByIdQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IBusinessRepository _businessRepository;

        public GetBusinessByIdQueryHandler(ILogger<GetBusinessByIdQueryHandler> logger, IMapper mapper, IBusinessRepository businessRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _businessRepository = businessRepository;
        }

        public Task<Result<BusinessReponse>> Handle(GetBusinessByIdQuery request, CancellationToken cancellationToken)
        {
            var business = _businessRepository.Include(b => b.Image)
                .FirstOrDefault(x => x.Id == request.BusinessId);

            if (business is null)
            {
                _logger.LogWarning("No se encontro el negocio con id: {BusinessId}", request.BusinessId);
                return Task.FromResult(Result.Fail<BusinessReponse>(new HttpError("No se encontro el negocio", ResultStatusCode.NoContent)));
            }

            var response = _mapper.Map<BusinessReponse>(business);
            _logger.LogInformation("Se encontro el negocio con id: {BusinessId}", request.BusinessId);
            return Task.FromResult(Result.Ok(response));
        }
    }
}
