using MeraStore.Services.Cross.Cutting.Domain.Entities;
using MeraStore.Shared.Kernel.Persistence.Interfaces;

namespace MeraStore.Services.Cross.Cutting.Application.Repositories;

public interface IRequestLogRepository : IRepository<RequestLog>
{
    // Define any additional methods specific to ResponseLog if needed
}

public interface IResponseLogRepository : IRepository<ResponseLog>
{
    // Define any additional methods specific to ResponseLog if needed
}