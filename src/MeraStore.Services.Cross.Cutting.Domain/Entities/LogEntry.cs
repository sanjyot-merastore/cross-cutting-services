using MeraStore.Shared.Kernel.Core.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Domain.Entities;

public abstract class LogEntry: Entity
{
  public byte[] Payload { get; set; } = []; // Raw request/response body
  public string ContentType { get; set; } = "application/json"; // Helps in decoding the payload
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  public string CorrelationId { get; set; } = string.Empty; // Links request and response
}