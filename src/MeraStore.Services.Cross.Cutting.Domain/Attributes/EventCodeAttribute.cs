using System.Diagnostics.CodeAnalysis;

namespace MeraStore.Services.Cross.Cutting.Domain.Attributes;

[ExcludeFromCodeCoverage]
public class EventCodeAttribute(string code): Attribute
{
    public string EventCode { get; } = code;
}
