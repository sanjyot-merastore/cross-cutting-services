using MeraStore.Shared.Kernel.Logging.Filters;
using MeraStore.Shared.Kernel.Logging.Interfaces;

namespace MeraStore.Services.Cross.Cutting.Api.Filters;

/// <summary>
/// 
/// </summary>
public static class MaskingFilterFactory
{
  public static IMaskingFilter ApiMaskingFilter()
  {
    // Configure request masking filter
    var requestFilter = new JsonPayloadRequestFilter();
    requestFilter.AddField("password");
    requestFilter.AddField("creditCardNumber");
    requestFilter.AddField("ssn");

    // Configure response masking filter
    var responseFilter = new JsonPayloadResponseFilter();
    responseFilter.AddField("password");
    responseFilter.AddField("creditCardNumber");
    responseFilter.AddField("ssn");
    responseFilter.AddField("summary");

    // Create MaskingFilter with both filters
    return new MaskingFilter(
      [requestFilter],
      [responseFilter]
    );
  }
}