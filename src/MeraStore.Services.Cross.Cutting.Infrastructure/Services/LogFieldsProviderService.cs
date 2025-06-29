using MeraStore.Services.Cross.Cutting.Application.Services;
using MeraStore.Services.Cross.Cutting.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeraStore.Services.Cross.Cutting.Infrastructure.Services;

public class LogFieldsProviderService : ILogFieldsProviderService
{
    private readonly ILogger<LogFieldsProviderService> _logger;
    private readonly Lazy<Task<LoggingFields>> _logFields;

    public LogFieldsProviderService(ILogger<LogFieldsProviderService> logger)
    {
        _logger = logger;
        _logFields = new Lazy<Task<LoggingFields>>(LoadFieldsAsync);
    }

    public async Task<LoggingFields> GetFieldsAsync()
    {
        return await _logFields.Value;
    }

    public async Task<LoggingFields> LoadFieldsAsync()
    {
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "configs", "log-schema.json");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("log-schema.json not found at {FilePath}", filePath);
                return new LoggingFields();
            }

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream);
            var jsonContent = await reader.ReadToEndAsync();

            var logFields = JsonConvert.DeserializeObject<LoggingFields>(jsonContent);
            return logFields ?? new LoggingFields();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading log-schema.json");
            return new LoggingFields();
        }
    }
}