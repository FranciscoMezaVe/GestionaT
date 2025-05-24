using FluentResults;
using MediatR;
using System;

namespace GestionaT.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(Guid Id, Guid BusinessId) : IRequest<Result>;
}
