using MeraStore.Services.Cross.Cutting.Application.Services;
using MeraStore.Services.Cross.Cutting.Domain.Entities;
using MeraStore.Services.Cross.Cutting.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeraStore.Services.Cross.Cutting.Api.Endpoints.Logging;

/// <summary>
/// Defines HTTP endpoints related to application logging operations,
/// including access to the logging field schema and reindexing support for observability tools.
/// </summary>
public class LoggingEndpoint : IEndpoint
{
    /// <summary>
    /// Maps all logging-related endpoints to the application's route builder under the <c>/logging</c> route group.
    /// </summary>
    /// <param name="app">The application's route builder used to define endpoints.</param>
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1.0/logging")
            .WithTags("Log Fields")
            .WithOpenApi();

        // GET /fields
        group.MapGet("/fields", async (
                [FromServices] ILogFieldsProviderService logService,
                [FromServices] ILogger<LogsEndpoint> logger,
                CancellationToken ct) =>
            {
                var fields = await logService.GetFieldsAsync();
                return Results.Ok(fields);
            })
            .WithName("GetLoggingFields")
            .WithSummary("Fetch the logging field definitions")
            .WithDescription("Returns the list of predefined logging fields used in structured logging for tools like Kibana, Elasticsearch, or Grafana.")
            .Produces<LoggingFields>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();

        // POST /re-index
        group.MapPost("/re-index", async (
                [FromServices] ILogIndexTemplateService logService,
                CancellationToken ct) =>
            {
                await logService.SetupTemplatesAsync();
                return Results.Ok();
            })
            .WithName("ReindexLoggingTemplates")
            .WithSummary("Reinitialize log index templates")
            .WithDescription("Triggers a setup process to reconfigure or update the log index templates in the logging backend (e.g., Elasticsearch). Useful after field schema changes.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();
    }
}