
using MeraStore.Services.Cross.Cutting.Api.Extensions;
using MeraStore.Services.Cross.Cutting.Api.Middlewares;
using MeraStore.Services.Cross.Cutting.Application;
using MeraStore.Services.Cross.Cutting.Domain;
using MeraStore.Services.Cross.Cutting.Infrastructure;
using MeraStore.Shared.Kernel.WebApi;
using MeraStore.Shared.Kernel.WebApi.Extensions;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeraStore.Services.Cross.Cutting.Api;

/// <summary>
/// 
/// </summary>
public class Program
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        string version = "v1";
        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();

        // JSON serialization config
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.WriteIndented = true;
        });


        builder.AddApiServices(Constants.ServiceName, defaultLogging: true);
        builder.Services.AddCoreApiServices(builder.Configuration);

        builder.AddLoggingServices();
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        app.UseCustomSwagger(Constants.ServiceName);
        app.UseMiddleware<TracingMiddleware>(Constants.ServiceName);
        app.UseMiddleware<ErrorHandlingMiddleware>();
        //app.UseMiddleware<LoggingMiddleware>();

        //Apply database migrations on startup with logging
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            await RunMigrations(logger, dbContext);
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapDiscoveredEndpoints();
        app.MapControllers();

        await app.RunAsync();
    }
    static async Task RunMigrations(ILogger<Program> logger, AppDbContext appDbContext)
    {
        try
        {
            logger.LogInformation("Applying database migrations...");
            await appDbContext.Database.MigrateAsync();
            logger.LogInformation("✅ Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error applying database migrations.");

        }
    }
}