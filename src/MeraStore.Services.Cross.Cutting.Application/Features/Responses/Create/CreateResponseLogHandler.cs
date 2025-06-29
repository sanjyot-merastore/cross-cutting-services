using MediatR;
using MeraStore.Services.Cross.Cutting.Application.Repositories;
using MeraStore.Services.Cross.Cutting.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Responses.Create;

public class CreateResponseLogHandler(IResponseLogRepository repository)
    : IRequestHandler<CreateResponseLogCommand, ResponseLog>
{
    public async Task<ResponseLog> Handle(CreateResponseLogCommand request, CancellationToken cancellationToken)
    {
        var entity = new ResponseLog
        {
            StatusCode = request.StatusCode,
            RequestId = request.RequestId,
            Payload = request.Payload,
            ContentType = request.ContentType,
            CorrelationId = request.CorrelationId,
            Timestamp = DateTime.UtcNow
        };

        var result = await repository.AddAsync(entity, cancellationToken);
        return result;
    }
}