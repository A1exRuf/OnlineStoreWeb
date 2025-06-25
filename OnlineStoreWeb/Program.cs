using Application;
using Application.Mapping;
using Infrastructure;
using OnlineStoreWeb.Extensions;
using OnlineStoreWeb.GuestCartMiddleware;
using OnlineStoreWeb.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<GuestCartMiddleware>();

MappingConfig.Configure();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<GuestCartMiddleware>();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
