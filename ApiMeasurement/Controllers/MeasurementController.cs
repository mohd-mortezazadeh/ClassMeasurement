using Microsoft.AspNetCore.Mvc;
using ApiMeasurement.PerfMeasurement;

namespace ApiMeasurement.Controllers;

/// <summary>
/// Controller that runs a target method under measurement and logs the result.
/// Use Postman to call the endpoints;
/// </summary>
[ApiController]
[Route("/")]
public class MeasurementController : ControllerBase
{
    /// <summary>
    /// POST /sync
    /// Runs a synchronous method (MyWorker.DoWork) under PerfMeasure.Measure,
    /// and returns the result to the caller.
    /// </summary>
    /// <returns>HTTP 200 OK with the PerfResult JSON payload.</returns>
    [HttpPost("sync")]
    public ActionResult<PerfResult> RunSync()
    {
        // Run the measured synchronous work and capture the PerfResult.
        // PerfMeasure.Measure takes an Action and returns a PerfResult with deltas.
        PerfResult result = PerfMeasure.Measure(() =>
        {
            var worker = new MyWorker(); // the class/method you want to measure
            worker.DoWork();
        });

        // Return the PerfResult to the HTTP client (Postman will receive JSON).
        return Ok(result.ToString());
    }

    /// <summary>
    /// POST /async
    /// Runs an asynchronous method (MyWorker.DoWorkAsync) under PerfMeasure.MeasureAsync,
    /// and returns the result to the caller.
    /// </summary>
    /// <returns>HTTP 200 OK with the PerfResult JSON payload.</returns>
    [HttpPost("async")]
    public async Task<ActionResult<PerfResult>> RunAsync()
    {
        // Measure an async workload. PerfMeasure.MeasureAsync accepts a Func<Task>.
        PerfResult result = await PerfMeasure.MeasureAsync(async () =>
        {
            var worker = new MyWorker();
            await worker.DoWorkAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);

        // Return the PerfResult to the HTTP client
        return Ok(result.ToString());
    }
}