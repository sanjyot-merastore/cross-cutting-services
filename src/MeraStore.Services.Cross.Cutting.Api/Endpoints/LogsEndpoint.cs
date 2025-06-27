using MediatR;
using MeraStore.Services.Cross.Cutting.Application.Features.Requests.Create;
using MeraStore.Services.Cross.Cutting.Application.Services;
using MeraStore.Services.Cross.Cutting.Domain.Entities;
using MeraStore.Services.Cross.Cutting.Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace MeraStore.Services.Cross.Cutting.Api.Endpoints;

/// <summary>
/// Defines HTTP endpoints related to application logging operations,
/// including access to the predefined logging field schema for Kibana or observability tools.
/// </summary>
public class LogsEndpoint : IEndpoint
{
    /// <summary>
    /// Maps all logging-related endpoints to the application's route builder under the <c>/logging</c> route group.
    /// </summary>
    /// <param name="app">The application's route builder used to define endpoints.</param>
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1.0/logs")
            .WithTags("Logs")
            .WithOpenApi();

        group.MapGet("/fields", async ([FromServices] ILogFieldsProvider logService, [FromServices] ILogger<LogsEndpoint> logger, CancellationToken ct) =>
        {
            //logger.LogInformation("Hello from log.");
            var fields = await logService.GetFieldsAsync();
            return Results.Ok(fields);
        })
            .WithName("GetLoggingFields")
            .WithSummary("Retrieves the logging fields schema")
            .WithDescription("Returns the predefined logging fields that are allowed in Kibana logs.")
            .Produces<LoggingFields>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();

        group.MapPost("/requests", async ([FromBody] CreateRequestLogCommand command, [FromServices] IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(command, ct);
                return Results.Created($"/logging/requests/{result.Id}", result);
            })
            .WithName("CreateApiRequestLog")
            .WithSummary("Creates a new API request log entry")
            .WithDescription("Stores details of an incoming API request, including HTTP method, URL, payload, and correlation ID.")
            .Produces<RequestLog>(StatusCodes.Status201Created, "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();

    }
}
