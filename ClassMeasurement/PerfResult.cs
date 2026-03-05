namespace ClassMeasurement;

/// <summary>
/// Represents the measured performance results for a single measurement run.
/// All memory values are stored in bytes; helper properties provide KB formatted values.
/// </summary>
public class PerfResult
{
    /// <summary>Elapsed wall-clock time for the measured action.</summary>
    public TimeSpan WallTime { get; init; }

    /// <summary>Delta of the process TotalProcessorTime during the measured action (after - before).</summary>
    public TimeSpan ProcessCpuTime { get; init; }

    /// <summary>Allocated bytes on the current managed thread during the measured action (delta: after - before).</summary>
    public long AllocatedBytesDelta { get; init; }

    /// <summary>GC.GetTotalMemory(false) value before the action (bytes).</summary>
    public long GcTotalMemoryBefore { get; init; }

    /// <summary>GC.GetTotalMemory(false) value after the action (bytes).</summary>
    public long GcTotalMemoryAfter { get; init; }

    /// <summary>Process.WorkingSet64 value before the action (bytes).</summary>
    public long WorkingSetBefore { get; init; }

    /// <summary>Process.WorkingSet64 value after the action (bytes).</summary>
    public long WorkingSetAfter { get; init; }

    /// <summary>Process thread count before the action.</summary>
    public int ThreadCountBefore { get; init; }

    /// <summary>Process thread count after the action.</summary>
    public int ThreadCountAfter { get; init; }

    /// <summary>GC total memory delta (after - before) in bytes.</summary>
    private long GcTotalMemoryDelta => GcTotalMemoryAfter - GcTotalMemoryBefore;

    /// <summary>Working set delta (after - before) in bytes.</summary>
    private long WorkingSetDelta => WorkingSetAfter - WorkingSetBefore;

    /// <summary>Thread count delta (after - before).</summary>
    private int ThreadCountDelta => ThreadCountAfter - ThreadCountBefore;

    /// <summary>Format bytes to human-readable KB string.</summary>
    private static string ToKb(long bytes) => $"{bytes / 1024.0:F2} KB";

    /// <summary>Return a short summary string for console output or logs.</summary>
    public override string ToString()
    {
        return
            $"Wall: {WallTime.TotalMilliseconds:F2} ms | " +
            $"CPU: {ProcessCpuTime.TotalMilliseconds:F2} ms | " +
            $"Alloc(thread): {ToKb(AllocatedBytesDelta)} | " +
            $"GC delta: {ToKb(GcTotalMemoryDelta)} | " +
            $"WorkingSet delta: {ToKb(WorkingSetDelta)} | " +
            $"Threads delta: {ThreadCountDelta}";
    }
}