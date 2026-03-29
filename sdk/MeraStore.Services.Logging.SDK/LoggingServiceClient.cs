using System.Net.Http.Json;
using MeraStore.Services.Logging.SDK.Interfaces;

namespace MeraStore.Services.Logging.SDK;

public class LoggingServiceClient(HttpClient httpClient) : ILoggingServiceClient
{
    public async Task<ApiResponse<RequestLog>> LogRequestAsync(RequestLog requestLog, IList<KeyValuePair<string, string>> headers = null)
        => await PostLogAsync(ApiEndpoints.RequestLogs.Create, requestLog, headers ?? []);

    public async Task<ApiResponse<ResponseLog>> LogResponseAsync(ResponseLog responseLog, IList<KeyValuePair<string, string>> headers = null)
        => await PostLogAsync(ApiEndpoints.ResponseLogs.Create, responseLog, headers ?? []);

    public async Task<ApiResponse<LoggingFields>> FetchLoggingFieldsAsync(IList<KeyValuePair<string, string>> headers = null)
        => await GetLogAsync<LoggingFields>(ApiEndpoints.FieldLogs.GetAll, headers: headers ?? []);

    public async Task<ApiResponse<string>> GetRequestPayloadByIdAsync(Ulid requestId, IList<KeyValuePair<string, string>> headers = null)
        => await GetLogAsync<string>(ApiEndpoints.RequestLogs.GetPayload, requestId, headers ?? []);

    public async Task<ApiResponse<string>> GetResponsePayloadByIdAsync(Ulid responseId, IList<KeyValuePair<string, string>> headers = null)
        => await GetLogAsync<string>(ApiEndpoints.ResponseLogs.GetPayload, responseId, headers ?? []);

    public async Task<ApiResponse<bool>> ReindexLogsAsync(IList<KeyValuePair<string, string>> headers = null)
    {
        AddHeaders(headers);
        var response = await httpClient.PostAsync(ApiEndpoints.ReIndexLogs, null);
        return await response.GetResponseOrFault<bool>();
    }

    private async Task<ApiResponse<T>> PostLogAsync<T>(string endpoint, T payload, IList<KeyValuePair<string, string>> headers = null) where T : BaseDto
    {
        AddHeaders(headers);
        var response = await httpClient.PostAsJsonAsync(endpoint, payload);
        return await response.GetResponseOrFault<T>();
    }

    private async Task<ApiResponse<T>> GetLogAsync<T>(string endpoint, Ulid? id = null, IList<KeyValuePair<string, string>> headers = null)
    {
        var url = id.HasValue ? string.Format(endpoint, id) : endpoint;
        AddHeaders(headers);
        var response = await httpClient.GetAsync(url);
        return await response.GetResponseOrFault<T>();
    }

    private void AddHeaders(IList<KeyValuePair<string, string>> headers)
    {
        if (headers is null || !headers.Any()) return;

        foreach (var header in headers)
        {
            if (!httpClient.DefaultRequestHeaders.Contains(header.Key))
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
}
