﻿using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

        public GetCustomerByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetCustomerByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomerResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = _unitOfWork.Repository<Customer>()
                .Include(c => c.Business)
                .FirstOrDefault(c => c.Id == request.Id && c.BusinessId == request.BusinessId);

            if (customer is null)
            {
                _logger.LogWarning("No se encontró el cliente {CustomerId} en el negocio {BusinessId}", request.Id, request.BusinessId);
                return Result.Fail(AppErrorFactory.NotFound(nameof(request.Id), request.Id));
            }

            var response = _mapper.Map<CustomerResponse>(customer);
            return Result.Ok(response);
        }
    }
}