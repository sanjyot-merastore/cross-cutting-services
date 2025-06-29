using MediatR;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Responses.GetPayloadById;

public class GetResponsePayloadByIdQuery(string id) : IRequest<byte[]>
{
    public string Id { get; set; } = id;
}