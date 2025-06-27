using MeraStore.Services.Cross.Cutting.Application.Repositories;
using MeraStore.Services.Cross.Cutting.Infrastructure.Repositories;
using MeraStore.Shared.Kernel.Persistence.Interfaces;
using MeraStore.Shared.Kernel.Persistence.Repositories;
using MeraStore.Shared.Kernel.Persistence.Strategy;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeraStore.Services.Cross.Cutting.Infrastructure
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext
            services.AddDbContext<AppDbContext>(op=>
                op.UseSqlServer(configuration.GetConnectionString("LoggingDb")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));

            services.AddScoped<ICommitStrategy, NoOpCommitStrategy>();

            services.AddScoped<IRequestLogRepository, RequestLogLogRepository>();
            services.AddScoped<IResponseLogRepository, ResponseLogRepository>();

            return services;
        }
    }
}