using FluentValidation;
using FluentResults;
using MediatR;
using GestionaT.Application.Common;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
        {
            var httpErrors = failures
                .Select(f => new HttpError(f.ErrorMessage, 400))
                .Cast<IError>()
                .ToList();

            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Fail(httpErrors);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse).GenericTypeArguments[0]);
                var failInstance = Activator.CreateInstance(resultType)!;

                var withErrorsMethod = resultType
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == "WithErrors" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(IEnumerable<IError>));

                withErrorsMethod?.Invoke(failInstance, new object[] { httpErrors });
                return (TResponse)failInstance;
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}