using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RentCarServer.Application.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;

namespace RentCarServer.Application
{
    public static class ServiceRegisterer
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceRegisterer).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(PermissionBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(ServiceRegisterer).Assembly);
            return services; 
        }
    }
}
