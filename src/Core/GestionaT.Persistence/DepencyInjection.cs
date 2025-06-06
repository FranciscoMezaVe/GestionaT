﻿using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Persistence.PGSQL;
using GestionaT.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestionaT.Persistence
{
    public static class DepencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            #region GENERAL
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            #endregion

            #region PGSQL
            var connectionString = Environment.GetEnvironmentVariable("PGSQLConnectionString");
            services.AddDbContext<AppPostgreSqlDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            #endregion

            return services;
        }
    }
}
