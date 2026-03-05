namespace ClassMeasurement;

/// <summary>
/// Example worker class whose method we will measure.
/// Replace or adapt this class with your real class under test.
/// </summary>
public class MyWorker
{
    /// <summary>
    /// Simulate CPU work and managed allocations.
    /// </summary>
    public void DoWork()
    {
        // Allocate a temporary buffer (managed)
        byte[] buffer = new byte[1024 * 200]; // ~200 KB

        // Do some CPU-bound work
        double acc = 0;
        for (int i = 0; i < 1_000_000; i++)
        {
            acc += Math.Sqrt(i);
        }

        // Use the buffer so the JIT doesn't optimize it away
        buffer[0] = (byte)(acc % 256);
    }

    /// <summary>
    /// Example async work that uses Task.Run to simulate thread-pool work.
    /// </summary>
    public async Task DoWorkAsync()
    {
        await Task.Run(() =>
        {
            // Simulate some work on a thread-pool thread
            var list = new List<int>();
            for (int i = 0; i < 100_000; i++) list.Add(i);

            // small CPU loop
            int s = 0;
            for (int i = 0; i < 200_000; i++) s += i & 1;
        }).ConfigureAwait(false);
    }
}