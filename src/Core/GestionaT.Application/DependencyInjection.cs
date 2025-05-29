using FluentValidation;
using GestionaT.Application.Behaviors;
using GestionaT.Application.Common.Errors.Abstractions;
using GestionaT.Application.Common.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GestionaT.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            services.AddAutoMapper(assembly);
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            services.AddHttpStatusCodeStrategies();

            return services;
        }

        private static IServiceCollection AddHttpStatusCodeStrategies(this IServiceCollection services)
        {
            services.AddScoped<IHttpStatusCodeStrategy, NotFoundStatusStrategy>();
            services.AddScoped<IHttpStatusCodeStrategy, AlreadyExistsStatusStrategy>();
            services.AddScoped<IHttpStatusCodeStrategy, ValidationStatusStrategy>();
            services.AddScoped<IHttpStatusCodeStrategy, UnauthorizedStatusStrategy>();
            services.AddScoped<IHttpStatusCodeStrategy, ForbiddenStatusStrategy>();
            services.AddScoped<IHttpStatusCodeStrategy, ConflictStatusStrategy>();
            services.AddScoped<IHttpStatusCodeStrategy, InternalErrorStatusStrategy>();

            services.AddScoped<HttpStatusCodeResolver>();

            return services;
        }
    }
}
