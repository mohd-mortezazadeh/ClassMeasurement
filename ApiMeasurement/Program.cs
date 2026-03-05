using ApiMeasurement.RequestPerf;

namespace ApiMeasurement;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        var app = builder.Build();
        
        app.UseRequestPerf();
        app.MapControllers();

        app.Run();
    }
}