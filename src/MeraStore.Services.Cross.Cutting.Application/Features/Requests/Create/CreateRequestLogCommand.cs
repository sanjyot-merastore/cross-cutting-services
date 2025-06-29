using MediatR;

using MeraStore.Services.Cross.Cutting.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Requests.Create;

public record CreateRequestLogCommand(string HttpMethod, string Url, byte[] Payload, string CorrelationId, string ContentType = "application/json") : IRequest<RequestLog>;