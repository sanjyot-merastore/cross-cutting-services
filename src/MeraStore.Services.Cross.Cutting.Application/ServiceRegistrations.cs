using MeraStore.Services.Cross.Cutting.Application.Features.Requests.Create;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeraStore.Services.Cross.Cutting.Application;

public static class ServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateRequestLogHandler).Assembly));

        return services;
    }
}