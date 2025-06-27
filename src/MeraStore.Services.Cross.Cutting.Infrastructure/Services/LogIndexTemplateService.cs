using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using MeraStore.Services.Cross.Cutting.Application.Services;
using Newtonsoft.Json;

namespace MeraStore.Services.Cross.Cutting.Infrastructure.Services
{
    public class LogIndexTemplateService(ElasticsearchClient client, ILogFieldsProvider loggingFieldService)
        : ILogIndexTemplateService
    {
        public async Task SetupTemplatesAsync()
        {
            var fields = await loggingFieldService.GetFieldsAsync();
            using var doc = JsonDocument.Parse(JsonConvert.SerializeObject(fields));

            var properties = doc.RootElement
                .GetProperty("mappings")
                .GetProperty("properties");
            var fieldMap = new Dictionary<string, string>();
            foreach (var prop in properties.EnumerateObject())
            {
                if (prop.Value.TryGetProperty("type", out var typeProp))
                    fieldMap[prop.Name] = typeProp.GetString()!;
            }


            var indexTemplates = new[]
            {
                "service-logs-template", "ef-logs-template", "infra-logs-template", "app-logs-template"
            };

            var indexPatterns = new[]
            {
                "service-logs-*", "ef-logs-*", "infra-logs-*", "app-logs-*"
            };

            for (var i = 0; i < indexTemplates.Length; i++)
            {
                var manager = new IndexTemplateManager(client, fieldMap, indexTemplates[i], indexPatterns[i]);
                await manager.PushAsync();
            }
        }
    }
}