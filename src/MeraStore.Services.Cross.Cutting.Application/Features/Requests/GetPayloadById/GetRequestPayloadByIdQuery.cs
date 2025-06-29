using MediatR;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Requests.GetPayloadById;

public class GetRequestPayloadByIdQuery(string id) : IRequest<byte[]>
{
    public string Id { get; set; } = id;
}