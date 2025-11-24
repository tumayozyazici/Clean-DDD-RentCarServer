using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RentCarServer.Infrastructure.Context;
using RentCarServer.Infrastructure.Options;
using Scrutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarServer.Infrastructure
{
    public static class ServiceRegisterer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.ConfigureOptions<JwtSetupOptions>();
            services.Configure<MailSettingOptions>(configuration.GetSection("MailSettings"));

            using var scoped = services.BuildServiceProvider().CreateScope();
            var mailSettings = scoped.ServiceProvider.GetRequiredService<IOptions<MailSettingOptions>>();
            if (string.IsNullOrEmpty(mailSettings.Value.UserId))
            {
                services.AddFluentEmail(mailSettings.Value.Email)
                        .AddSmtpSender(
                        mailSettings.Value.Smtp,
                        mailSettings.Value.Port);
            }
            else
            {
                services.AddFluentEmail(mailSettings.Value.Email)
                    .AddSmtpSender(
                    mailSettings.Value.Smtp,
                    mailSettings.Value.Port,
                    mailSettings.Value.UserId,
                    mailSettings.Value.Password);
            }

            services.AddAuthentication().AddJwtBearer();
            services.AddAuthorization();


            string con = configuration.GetConnectionString("SqlServer")!;
            services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(con));
            services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());
            services.Scan(scan => scan
                .FromAssemblies(typeof(ServiceRegisterer).Assembly)
                .AddClasses(publicOnly: false)
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
