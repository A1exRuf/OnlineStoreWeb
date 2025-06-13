using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace OnlineStoreWeb.Configuration
{
    public static class ApplicationDbContextConfig
    {
        public static void AddApplicationDbContextConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(builder =>
            builder.UseNpgsql(configuration.GetConnectionString("Database")));
        }
    }
}
