using Application.Abstractions;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(builder => 
            builder.UseNpgsql(configuration.GetConnectionString("Database")));

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenProvider, TokenProvider>();

            return services;
        }
    }
}
