using GestionaT.Application.Behaviors;
using GestionaT.Application.Interfaces.Auth;
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
            });

            return services;
        }
    }
}
