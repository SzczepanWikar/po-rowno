﻿using Infrastructure.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseProjections
{
    public static class Configuration
    {
        public static IServiceCollection AddDatabaseProjections(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddProjections<ApplicationContext>(config);
            return services;
        }
    }
}
