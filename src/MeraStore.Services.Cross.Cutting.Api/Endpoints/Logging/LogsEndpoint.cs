using MediatR;

using MeraStore.Services.Cross.Cutting.Application.Features.Requests.Create;
using MeraStore.Services.Cross.Cutting.Application.Features.Requests.GetPayloadById;
using MeraStore.Services.Cross.Cutting.Application.Features.Responses.Create;
using MeraStore.Services.Cross.Cutting.Application.Features.Responses.GetPayloadById;
using MeraStore.Services.Cross.Cutting.Application.Services;
using MeraStore.Services.Cross.Cutting.Domain.Entities;
using MeraStore.Services.Cross.Cutting.Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace MeraStore.Services.Cross.Cutting.Api.Endpoints.Logging;

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
        var group = app.MapGroup("api/v1.0/logging")
            .WithTags("Logs")
            .WithOpenApi();

        // POST /requests
        group.MapPost("/requests", async (
            [FromBody] CreateRequestLogCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/logging/requests/payload/{result.Id}", result);
        })
        .WithName("CreateApiRequestLog")
        .WithSummary("Creates a new API request log entry")
        .WithDescription("Stores details of an incoming API request, including HTTP method, URL, payload, and correlation ID.")
        .Produces<RequestLog>(StatusCodes.Status201Created, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();

        // GET /requests/payload/{id}
        group.MapGet("/requests/payload/{id}", async (
            string id,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetRequestPayloadByIdQuery(id);
            var payload = await mediator.Send(query, ct);

            return payload != null
                ? Results.Bytes(payload, "application/json")
                : Results.NotFound();
        })
        .WithName("GetRequestPayload")
        .WithSummary("Retrieves the payload of a logged API request")
        .WithDescription("Returns the payload of an API request log by its unique identifier.")
        .Produces<byte[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();

        // POST /responses
        group.MapPost("/responses", async (
            [FromBody] CreateResponseLogCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/logging/responses/payload/{result.Id}", result);
        })
        .WithName("CreateApiResponseLog")
        .WithSummary("Creates a new API response log entry")
        .WithDescription("Stores details of an outgoing API response, including status code, payload, and correlation ID.")
        .Produces<ResponseLog>(StatusCodes.Status201Created, "application/json")
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();

        // GET /responses/payload/{id}
        group.MapGet("/responses/payload/{id}", async (
            string id,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetResponsePayloadByIdQuery(id); // NEW QUERY!
            var payload = await mediator.Send(query, ct);

            return payload != null
                ? Results.Bytes(payload, "application/json")
                : Results.NotFound();
        })
        .WithName("GetResponsePayload") // ✅ unique name
        .WithSummary("Retrieves the payload of a logged API response")
        .WithDescription("Returns the payload of an API response log by its unique identifier.")
        .Produces<byte[]>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();
    }
}
