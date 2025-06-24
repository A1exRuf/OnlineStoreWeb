using Application.Abstractions;
using Azure.Storage.Blobs;
using Domain.Abstractions;
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

            var blobStorageConnection = configuration.GetConnectionString("BlobStorage");
            var blobStorageOptions = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2019_12_12);
            services.AddSingleton(_ => new BlobServiceClient(blobStorageConnection, blobStorageOptions));
            services.AddSingleton<IBlobService, BlobService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenProvider, TokenProvider>();

            return services;
        }
    }
}
