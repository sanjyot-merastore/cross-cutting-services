namespace MeraStore.Services.Cross.Cutting.Domain.Entities;

public class RequestLog : LogEntry
{
  public string HttpMethod { get; set; } = "GET";
  public string Url { get; set; } = string.Empty;
}