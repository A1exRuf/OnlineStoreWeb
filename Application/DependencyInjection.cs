using Application.Abstractions;
using Application.Behaviors;
using Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            });
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services
                .AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
                .AddSmtpSender(configuration["Email:Host"], configuration.GetValue<int>("Email:Port"));

            services.AddScoped<ICategoryTreeBuilder, CategoryTreeBuilder>();

            return services;
        }
    }
}
