using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using MeraStore.Services.Cross.Cutting.Application.Services;
using Newtonsoft.Json;

namespace MeraStore.Services.Cross.Cutting.Infrastructure.Services;

public class LogIndexTemplateService(ElasticsearchClient client, ILogFieldsProviderService loggingFieldService)
    : ILogIndexTemplateService
{
    public async Task SetupTemplatesAsync()
    {
        var fields = await loggingFieldService.GetFieldsAsync();

        using var doc = JsonDocument.Parse(JsonConvert.SerializeObject(fields));
        var properties = doc.RootElement
            .GetProperty("mappings")
            .GetProperty("properties");

        var fieldMap = properties.EnumerateObject()
            .Where(p => p.Value.TryGetProperty("type", out _))
            .ToDictionary(p => p.Name, p => p.Value.GetProperty("type").GetString()!);

        var templateConfigs = new (string Template, string Pattern)[]
        {
            ("ef-logs-template", "ef-logs-*"),
            ("infra-logs-template", "infra-logs-*"),
            ("app-logs-template", "app-logs-*"),
            //("service-logs-template", "service-logs-*")
        };

        var tasks = templateConfigs.Select(config =>
        {
            var manager = new IndexTemplateManager(client, fieldMap, config.Template, config.Pattern);
            return manager.PushAsync();
        });

        await Task.WhenAll(tasks);
    }
}
