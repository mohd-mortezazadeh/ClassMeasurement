namespace ClassMeasurement;

/// <summary>
/// Console program demonstrating usage of PerfMeasure for multiple iterations
/// </summary>
static class Program
{
    static async Task Main()
    {
        const int iterations = 5; // increase to 50-100 for more stable numbers
        var syncResults = new List<PerfResult>();
        var asyncResults = new List<PerfResult>();

        Console.WriteLine(new string('-', 80));
        
        // Measure synchronous DoWork
        for (int i = 0; i < iterations; i++)
        {
            var res = PerfMeasure.Measure(() =>
            {
                var w = new MyWorker();
                w.DoWork();
            });
            syncResults.Add(res);
            Console.WriteLine($"Sync run {i + 1}: {res}");
        }

        Console.WriteLine(new string('-', 80));

        // Measure asynchronous DoWorkAsync
        for (int i = 0; i < iterations; i++)
        {
            var res = await PerfMeasure.MeasureAsync(async () =>
            {
                var w = new MyWorker();
                await w.DoWorkAsync();
            });
            asyncResults.Add(res);
            Console.WriteLine($"Async run {i + 1}: {res}");
        }

        Console.WriteLine(new string('-', 80));
    }
}