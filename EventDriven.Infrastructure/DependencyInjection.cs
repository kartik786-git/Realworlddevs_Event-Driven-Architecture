using EventDriven.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDriven.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEventDrivenInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
            services.AddSingleton<IEventBus, InMemoryEventBus>();
            return services;
        }
    }
}
