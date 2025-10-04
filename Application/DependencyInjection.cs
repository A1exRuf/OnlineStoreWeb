using Application.Abstractions;
using Application.Abstractions.Carts;
using Application.Abstractions.Users.CartInitialization;
using Application.Behaviors;
using Application.Services.Carts;
using Application.Services.Categories;
using Application.Services.Users.CartInitialization;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            });
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped<ICategoryTreeBuilder, CategoryTreeBuilder>();

            services.AddScoped<ICartServiceFactory, CartServiceFactory>();
            services.AddScoped<CustomerCartService>();
            services.AddScoped<GuestCartService>();

            services.AddScoped<ICartInitializationStrategyFactory, CartInitializationStrategyFactory>();
            services.AddScoped<NewCartInitializationStrategy>();
            services.AddScoped<TransferGuestCartInitializationStrategy>();
            

            return services;
        }
    }
}
