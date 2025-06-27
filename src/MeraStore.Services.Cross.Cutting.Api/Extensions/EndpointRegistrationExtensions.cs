using System.Reflection;
using MeraStore.Services.Cross.Cutting.Domain.Interfaces;

namespace MeraStore.Services.Cross.Cutting.Api.Extensions;

/// <summary>
/// Provides extension methods for dynamically registering and mapping API endpoints
/// that implement the <see cref="IEndpoint"/> interface.
/// </summary>
public static class EndpointRegistrationExtensions
{
    /// <summary>
    /// Resolves all registered <see cref="IEndpoint"/> implementations from the service provider
    /// and invokes their <c>MapEndpoints</c> method to register their routes to the application.
    /// </summary>
    /// <param name="app">The endpoint route builder used to define API routes.</param>
    public static void MapDiscoveredEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointTypes = Assembly.GetExecutingAssembly()
            .ExportedTypes
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();

        foreach (var endpoint in endpointTypes)
        {
            endpoint.MapEndpoints(app);
        }
    }
}