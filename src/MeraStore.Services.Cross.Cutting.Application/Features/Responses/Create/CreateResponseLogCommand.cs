using MediatR;
using MeraStore.Services.Cross.Cutting.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Responses.Create;

public record CreateResponseLogCommand(
    int StatusCode,
    Guid RequestId,
    byte[] Payload,
    string CorrelationId,
    string ContentType = "application/json") : IRequest<ResponseLog>;
