using MediatR;

using MeraStore.Services.Cross.Cutting.Application.Repositories;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Requests.GetPayloadById;

public class GetRequestPayloadByIdQueryHandler(IRequestLogRepository repository)
    : IRequestHandler<GetRequestPayloadByIdQuery, byte[]>
{

    public async Task<byte[]> Handle(GetRequestPayloadByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity?.Payload;
    }
}