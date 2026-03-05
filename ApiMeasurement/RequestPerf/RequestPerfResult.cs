namespace ApiMeasurement.RequestPerf;

/// <summary>
/// DTO that holds per-request performance measurement results.
/// All memory values are stored in bytes; helper properties format KB.
/// </summary>
public class RequestPerfResult
{
    /// <summary>HTTP method (GET, POST, etc.).</summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>Request path (URL path).</summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>HTTP response status code.</summary>
    public int StatusCode { get; init; }

    /// <summary>Elapsed wall-clock time for the request.</summary>
    public TimeSpan WallTime { get; init; }

    /// <summary>Delta of the process TotalProcessorTime during the request (after - before).</summary>
    public TimeSpan ProcessCpuTime { get; init; }

    /// <summary>Allocated managed bytes on the request's managed thread (delta: after - before).</summary>
    public long AllocatedBytesDelta { get; init; }

    /// <summary>GC.GetTotalMemory(false) before the request (bytes).</summary>
    public long GcTotalMemoryBefore { get; init; }

    /// <summary>GC.GetTotalMemory(false) after the request (bytes).</summary>
    public long GcTotalMemoryAfter { get; init; }

    /// <summary>Process.WorkingSet64 before the request (bytes).</summary>
    public long WorkingSetBefore { get; init; }

    /// <summary>Process.WorkingSet64 after the request (bytes).</summary>
    public long WorkingSetAfter { get; init; }

    /// <summary>OS thread count before the request.</summary>
    public int ThreadCountBefore { get; init; }

    /// <summary>OS thread count after the request.</summary>
    public int ThreadCountAfter { get; init; }

    /// <summary>GC total memory delta (after - before) in bytes.</summary>
    private long GcTotalMemoryDelta => GcTotalMemoryAfter - GcTotalMemoryBefore;

    /// <summary>Working set delta (after - before) in bytes.</summary>
    private long WorkingSetDelta => WorkingSetAfter - WorkingSetBefore;

    /// <summary>Thread count delta (after - before).</summary>
    private int ThreadCountDelta => ThreadCountAfter - ThreadCountBefore;

    /// <summary>Format bytes to KB string.</summary>
    private static string ToKb(long bytes) => $"{bytes / 1024.0:F2} KB";

    /// <summary>Return a short summary string suitable for logs.</summary>
    public override string ToString()
    {
        return
            $"Method={Method} Path={Path} Status={StatusCode} " +
            $"Wall: {WallTime.TotalMilliseconds:F2} ms | " +
            $"CPU: {ProcessCpuTime.TotalMilliseconds:F2} ms | " +
            $"Alloc(thread): {ToKb(AllocatedBytesDelta)} | " +
            $"GC delta: {ToKb(GcTotalMemoryDelta)} | " +
            $"WorkingSet delta: {ToKb(WorkingSetDelta)} | " +
            $"Threads delta: {ThreadCountDelta}";
    }
}