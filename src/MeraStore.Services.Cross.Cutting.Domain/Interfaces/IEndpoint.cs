using Microsoft.AspNetCore.Routing;

namespace MeraStore.Services.Cross.Cutting.Domain.Interfaces;

public interface IEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
