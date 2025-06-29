using Serilog.Events;

namespace MeraStore.Services.Cross.Cutting.Domain.LogSinks;

public class EfLogsElasticsearchSink(string elasticsearchUrl)
  : BaseElasticsearchSink(elasticsearchUrl, Constants.Logging.Elasticsearch.EntityFrameworkCoreIndexFormat)
{
  public override void Emit(LogEvent logEvent)
  {
    var logEntry = GetCommonLogFields(logEvent);
    var sourceContext = logEntry[Constants.Logging.LogFields.SourceContext]?.ToString();

    if (sourceContext == null || !sourceContext.Contains("Microsoft.EntityFrameworkCore"))
      return; // Ignore logs that don't belong here

    Task.Run(async () => await Client.IndexAsync(logEntry, idx => idx.Index($"{Constants.Logging.Elasticsearch.EntityFrameworkCoreIndexFormat}{DateTime.UtcNow:yyyy-MM}")));
  }
}