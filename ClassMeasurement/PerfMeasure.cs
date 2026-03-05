using System.Diagnostics;

namespace ClassMeasurement;

/// <summary>
/// Lightweight cross-platform performance measurement helper.
/// Use Measure(Action) for synchronous code and MeasureAsync(Func<Task>) for async code.
/// The helper takes before/after snapshots and returns a PerfResult with deltas.
/// </summary>
public static class PerfMeasure
{
    /// <summary>
    /// Measure a synchronous action. Returns a PerfResult with before/after deltas.
    /// Keep the action focused to reduce noise.
    /// </summary>
    /// <param name="action">The action to measure.</param>
    /// <returns>PerfResult containing wall time, CPU delta, allocation delta, memory and thread deltas.</returns>
    public static PerfResult Measure(Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var proc = Process.GetCurrentProcess();

        // Snapshot before values
        long allocBefore = GC.GetAllocatedBytesForCurrentThread(); // managed allocations on this thread
        long gcBefore = GC.GetTotalMemory(false);                  // managed heap size (approx)
        long wsBefore = proc.WorkingSet64;                         // resident memory (bytes)
        int threadsBefore = proc.Threads.Count;                    // OS thread count
        TimeSpan cpuBefore = proc.TotalProcessorTime;              // process CPU time

        var sw = Stopwatch.StartNew();

        // Execute measured action
        action();

        sw.Stop();

        // Snapshot after values
        TimeSpan cpuAfter = proc.TotalProcessorTime;
        long allocAfter = GC.GetAllocatedBytesForCurrentThread();
        long gcAfter = GC.GetTotalMemory(false);
        long wsAfter = proc.WorkingSet64;
        int threadsAfter = proc.Threads.Count;

        return new PerfResult
        {
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
    }

    /// <summary>
    /// Measure an asynchronous function. Note: allocations measured are for the thread
    /// that executes the snapshot calls; async/await may resume on different threads,
    /// so allocation deltas can be noisy for highly asynchronous code.
    /// </summary>
    /// <param name="func">Async function to measure.</param>
    /// <returns>PerfResult with deltas.</returns>
    public static async Task<PerfResult> MeasureAsync(Func<Task> func)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));

        var proc = Process.GetCurrentProcess();

        long allocBefore = GC.GetAllocatedBytesForCurrentThread();
        long gcBefore = GC.GetTotalMemory(false);
        long wsBefore = proc.WorkingSet64;
        int threadsBefore = proc.Threads.Count;
        TimeSpan cpuBefore = proc.TotalProcessorTime;
        var sw = Stopwatch.StartNew();

        await func().ConfigureAwait(false);

        sw.Stop();

        TimeSpan cpuAfter = proc.TotalProcessorTime;
        long allocAfter = GC.GetAllocatedBytesForCurrentThread();
        long gcAfter = GC.GetTotalMemory(false);
        long wsAfter = proc.WorkingSet64;
        int threadsAfter = proc.Threads.Count;

        return new PerfResult
        {
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
    }
}