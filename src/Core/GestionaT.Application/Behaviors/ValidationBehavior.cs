using FluentValidation;
using FluentResults;
using MediatR;
using GestionaT.Application.Common.Errors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : IResultBase
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => AppErrorFactory.Validation(e.PropertyName, e.ErrorMessage))
            .ToList();

        if (failures.Any())
        {
            // Manejo simple para Result o Result<T>
            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Fail(failures);

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Asumimos que el tipo es Result<T>, retornamos Result<T>.Fail(...)
                var genericType = typeof(TResponse).GetGenericArguments()[0];
                var failMethod = typeof(Result)
                    .GetMethod(nameof(Result.Fail), new[] { typeof(IEnumerable<Error>) })!
                    .MakeGenericMethod(genericType);
                var result = failMethod.Invoke(null, new object[] { failures })!;
                return (TResponse)result;
            }

            throw new InvalidOperationException("Tipo de respuesta no soportado por ValidationBehavior.");
        }

        return await next();
    }
}
