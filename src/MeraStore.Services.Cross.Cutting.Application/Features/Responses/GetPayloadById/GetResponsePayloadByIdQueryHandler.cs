using MediatR;

using MeraStore.Services.Cross.Cutting.Application.Repositories;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Responses.GetPayloadById;

public class GetResponsePayloadByIdQueryHandler(IResponseLogRepository repository)
    : IRequestHandler<GetResponsePayloadByIdQuery, byte[]>
{


    public async Task<byte[]> Handle(GetResponsePayloadByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity?.Payload;
    }
}