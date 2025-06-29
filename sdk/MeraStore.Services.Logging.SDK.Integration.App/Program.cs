// See https://aka.ms/new-console-template for more information

using System.Text;
using Bogus;
using MeraStore.Services.Logging.SDK;
using MeraStore.Services.Logging.SDK.Integration.App;
using MeraStore.Services.Logging.SDK.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


// Setup DI container
var serviceProvider = new ServiceCollection()
    .AddSingleton<ClientBuilder>()
    .BuildServiceProvider();

var clientBuilder = serviceProvider.GetRequiredService<ClientBuilder>();

// Configure the client
var loggingClient = clientBuilder
    .WithUrl("http://logging-api.merastore.com:8101/") // Replace with actual service URL
    .UseDefaultResiliencePolicy()
    .Build();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nLogging SDK Test Menu");
    Console.WriteLine("1. Create Request Log");
    Console.WriteLine("2. Get Request Payload");
    Console.WriteLine("3. Create Response Log");
    Console.WriteLine("4. Get Response Payload");
    Console.WriteLine("5. Get Logging Fields");
    Console.WriteLine("6. Reindex Logs");
    Console.WriteLine("7. Exit");
    Console.Write("Enter your choice: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            await ExecuteWithColor(() => CreateRequestLog(loggingClient));
            break;
        case "2":
            await ExecuteWithColor(() => GetRequestPayload(loggingClient));
            break;
        case "3":
            await ExecuteWithColor(() => CreateResponseLog(loggingClient));
            break;
        case "4":
            await ExecuteWithColor(() => GetResponsePayload(loggingClient));
            break;
        case "5":
            await ExecuteWithColor(() => GetLoggingFields(loggingClient));
            break;
        case "6":
            await ExecuteWithColor(() => ReIndexLogs(loggingClient));
            break;
        case "7":
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid choice, please try again.");
            break;
    }
}



static async Task CreateRequestLog(LoggingServiceClient loggingClient)
{

  var requestLog = new RequestLog()
  {
    CorrelationId = Guid.NewGuid().ToString(),
    Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Student.GetFakeStudent(), Formatting.Indented)),
    Timestamp = DateTime.UtcNow,
    HttpMethod = HttpMethod.Post.ToString(),
    Url = new Faker().Internet.Url()
  };
  Console.ForegroundColor = ConsoleColor.Green;
  var response = await loggingClient.LogRequestAsync(requestLog, GetDefaultHeaders());
  var requestId = response.Response?.Id;

  Console.WriteLine($"Created Request Log with ID: {requestId}");
}


static async Task GetRequestPayload(LoggingServiceClient loggingClient)
{
  Console.WriteLine("Enter Request Log ID: ");
  if (Ulid.TryParse(Console.ReadLine(), out var requestId))
  {
    Console.ForegroundColor = ConsoleColor.Green;
    var retrievedRequestLog = await loggingClient.GetRequestPayloadByIdAsync(requestId, GetDefaultHeaders());
    Console.WriteLine($"Request:");
    Console.WriteLine(JsonConvert.SerializeObject(retrievedRequestLog.Response, Formatting.Indented));
  }
  else
  {
    Console.WriteLine("Invalid ID format.");
  }
}

static async Task CreateResponseLog(LoggingServiceClient loggingClient)
{
  var responseLog = new ResponseLog()
  {
    CorrelationId = Guid.NewGuid().ToString(),
    Payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Student.GetFakeStudent(), Formatting.Indented)),
    Timestamp = DateTime.UtcNow,
    RequestId = Guid.NewGuid(),
    StatusCode = 200
  };
  var response = await loggingClient.LogResponseAsync(responseLog, GetDefaultHeaders());
  var requestId = response.Response?.Id;
  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine($"Created Response Log with ID: {requestId}");

}

static async Task GetResponsePayload(LoggingServiceClient loggingClient)
{
  Console.WriteLine("Enter Response Log ID: ");
  if (Ulid.TryParse(Console.ReadLine(), out var responseId))
  {
    var retrievedResponseLog = await loggingClient.GetResponsePayloadByIdAsync(responseId, GetDefaultHeaders());
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Response:");
    Console.WriteLine(JsonConvert.SerializeObject(retrievedResponseLog.Response, Formatting.Indented));
  }
  else
  {
    Console.WriteLine("Invalid ID format.");
  }
}

static async Task GetLoggingFields(LoggingServiceClient loggingClient)
{
  var logFields = await loggingClient.FetchLoggingFieldsAsync(GetDefaultHeaders());
  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine("Logging Fields: " + string.Join(", ", JsonConvert.SerializeObject(logFields.Response)));
}
static async Task ReIndexLogs(LoggingServiceClient loggingClient)
{
  var logFields = await loggingClient.ReindexLogsAsync(GetDefaultHeaders());
  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine("Logging Fields: " + string.Join(", ", JsonConvert.SerializeObject(logFields.Response)));
}

static async Task ExecuteWithColor(Func<Task> action)
{
  try
  {
    Console.ForegroundColor = ConsoleColor.Green;
    await action();
  }
  catch (Exception ex)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
  }
  finally
  {
    Console.ResetColor();
  }
}

static IList<KeyValuePair<string, string>> GetDefaultHeaders()
{
  return new List<KeyValuePair<string, string>>()
  {
    new KeyValuePair<string, string>("ms-correlationId", Guid.NewGuid().ToString())
  };
}