namespace MeraStore.Services.Logging.SDK;

public static class ApiEndpoints
{
  public const string BaseUri = "/api/v1.0/logging"; // Base URI for the API
  public const string ReIndexLogs = "/api/v1.0/logging"; // Base URI for the API

  public static class FieldLogs
  {
    public const string GetAll = $"{BaseUri}/fields";
  }

  public static class RequestLogs
  {
    public const string Create = $"{BaseUri}/requests";
    public const string GetPayload = $"{BaseUri}/requests/payload/{{0}}"; // Placeholder for ID
  }

  public static class ResponseLogs
  {
    public const string Create = $"{BaseUri}/responses";
    public const string GetPayload = $"{BaseUri}/responses/payload/{{0}}"; // Placeholder for ID
  }
}