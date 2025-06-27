namespace MeraStore.Services.Cross.Cutting.Domain.Entities;

public class ResponseLog : LogEntry
{
  public int StatusCode { get; set; }
  public Guid RequestId { get; set; } // Link back to the original request
}