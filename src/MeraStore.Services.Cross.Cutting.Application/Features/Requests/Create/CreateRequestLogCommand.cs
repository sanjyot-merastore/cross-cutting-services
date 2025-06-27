using MediatR;
using MeraStore.Services.Cross.Cutting.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Requests.Create;

public class CreateRequestLogCommand : IRequest<RequestLog>
{
    public string HttpMethod { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public byte[] Payload { get; set; } = [];
    public string ContentType { get; set; } = "application/json";
    public string CorrelationId { get; set; } = string.Empty;
}