namespace MeraStore.Services.Logging.SDK.Interfaces;

public interface ILoggingServiceClient
{
    Task<ApiResponse<RequestLog>> LogRequestAsync(RequestLog requestLog, IList<KeyValuePair<string, string>> headers = null);

    Task<ApiResponse<ResponseLog>> LogResponseAsync(ResponseLog responseLog, IList<KeyValuePair<string, string>> headers = null);

    Task<ApiResponse<LoggingFields>> FetchLoggingFieldsAsync(IList<KeyValuePair<string, string>> headers = null);

    Task<ApiResponse<string>> GetRequestPayloadByIdAsync(Ulid requestId, IList<KeyValuePair<string, string>> headers = null);

    Task<ApiResponse<string>> GetResponsePayloadByIdAsync(Ulid responseId, IList<KeyValuePair<string, string>> headers = null);

    Task<ApiResponse<bool>> ReindexLogsAsync(IList<KeyValuePair<string, string>> headers = null);
}