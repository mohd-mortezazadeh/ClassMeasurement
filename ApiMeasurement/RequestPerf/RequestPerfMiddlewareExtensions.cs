namespace ApiMeasurement.RequestPerf;

/// <summary>
/// Extension methods to register RequestPerfMiddleware in the ASP.NET Core pipeline.
/// </summary>
public static class RequestPerfMiddlewareExtensions
{
    /// <summary>
    /// Adds RequestPerfMiddleware to the pipeline.
    /// Place this early to capture full request handling cost.
    /// </summary>
    public static IApplicationBuilder UseRequestPerf(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestPerfMiddleware>();
    }
}