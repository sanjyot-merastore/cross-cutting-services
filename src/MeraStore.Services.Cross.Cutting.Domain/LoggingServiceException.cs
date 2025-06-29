using System.Net;

using MeraStore.Shared.Kernel.Exceptions;
using MeraStore.Shared.Kernel.Exceptions.Codes.Events;
using MeraStore.Shared.Kernel.Exceptions.Codes.Http;

namespace MeraStore.Services.Cross.Cutting.Domain;

public static class LoggingException
{

    public static LoggingServiceException LogReindexingFailed(string message, Exception? innerException = null)
    {
        EventCodeRegistry.Register("ReIndexingFailed", "301");
        ErrorCodeRegistry.Register("ReIndexingFailed", "301");

        return new LoggingServiceException(EventCodeRegistry.GetCode("ReIndexingFailed"), ErrorCodeRegistry.GetCode("ReIndexingFailed"),
            HttpStatusCode.InternalServerError, message ?? $"Reindexing operation failed in the logging service."
        );
    }

}