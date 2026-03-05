using System.Diagnostics;

namespace ApiMeasurement.RequestPerf;

/// <summary>
/// ASP.NET Core middleware that measures per-request performance and logs a RequestPerfResult.
/// Place this middleware early in the pipeline to capture the full request handling cost.
/// </summary>
public class RequestPerfMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerfMiddleware> _logger;

    /// <summary>
    /// Create a new instance of the middleware.
    /// </summary>
    /// <param name="next">Next middleware in the pipeline.</param>
    /// <param name="logger">Logger used to emit structured logs.</param>
    public RequestPerfMiddleware(RequestDelegate next, ILogger<RequestPerfMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Called per HTTP request. Measures before/after snapshots and logs the result.
    /// Sampling is applied according to RequestPerfOptions.SamplingRate.
    /// </summary>
    /// <param name="context">HttpContext for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var proc = Process.GetCurrentProcess();

        // Snapshot before values
        long allocBefore = GC.GetAllocatedBytesForCurrentThread(); // allocations on current managed thread
        long gcBefore = GC.GetTotalMemory(false);                  // managed heap estimate
        long wsBefore = proc.WorkingSet64;                         // resident memory
        int threadsBefore = proc.Threads.Count;                    // OS thread count
        TimeSpan cpuBefore = proc.TotalProcessorTime;              // process CPU time
        var sw = Stopwatch.StartNew();                             // wall-clock timer

        // Execute the rest of the pipeline (controllers, other middleware)
        await _next(context).ConfigureAwait(false);

        // Stop timers and take after snapshots
        sw.Stop();
        TimeSpan cpuAfter = proc.TotalProcessorTime;
        long allocAfter = GC.GetAllocatedBytesForCurrentThread();
        long gcAfter = GC.GetTotalMemory(false);
        long wsAfter = proc.WorkingSet64;
        int threadsAfter = proc.Threads.Count;

        // Build result DTO
        var result = new RequestPerfResult
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            StatusCode = context.Response?.StatusCode ?? 0,
            WallTime = sw.Elapsed,
            ProcessCpuTime = cpuAfter - cpuBefore,
            AllocatedBytesDelta = allocAfter - allocBefore,
            GcTotalMemoryBefore = gcBefore,
            GcTotalMemoryAfter = gcAfter,
            WorkingSetBefore = wsBefore,
            WorkingSetAfter = wsAfter,
            ThreadCountBefore = threadsBefore,
            ThreadCountAfter = threadsAfter
        };

        // Emit a structured log line. Adjust log level or sink as needed.
        _logger.LogInformation(result.ToString());
    }
}