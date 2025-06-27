using MediatR;
using MeraStore.Services.Cross.Cutting.Application.Repositories;
using MeraStore.Services.Cross.Cutting.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Application.Features.Requests.Create;

public class CreateRequestLogHandler(IRequestLogRepository repository)
    : IRequestHandler<CreateRequestLogCommand, RequestLog>
{
    public async Task<RequestLog> Handle(CreateRequestLogCommand request, CancellationToken cancellationToken)
    {
        var entity = new RequestLog
        {
            HttpMethod = request.HttpMethod,
            Url = request.Url,
            Payload = request.Payload,
            ContentType = request.ContentType,
            CorrelationId = request.CorrelationId,
            Timestamp = DateTime.UtcNow
        };

        var result = await repository.AddAsync(entity, cancellationToken);
        return result;
    }
}