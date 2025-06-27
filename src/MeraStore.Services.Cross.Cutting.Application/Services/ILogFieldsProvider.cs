using MeraStore.Services.Cross.Cutting.Domain.Entities;

namespace MeraStore.Services.Cross.Cutting.Application.Services;

public interface ILogFieldsProvider
{
  Task<LoggingFields> GetFieldsAsync();
}