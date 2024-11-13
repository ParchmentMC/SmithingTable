using Microsoft.AspNetCore.HttpLogging;
using OpenTelemetry.Metrics;
using SmithingTable.HealthChecks;
using SmithingTable.Services;
using SmithingTable.Worker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddSingleton<IParchmentVersionUpdater, ParchmentVersionUpdater>();
builder.Services.AddSingleton<IParchmentVersionService, ParchmentVersionService>();
builder.Services.AddHostedService<ParchmentVersionRetrievalWorker>();
builder.Services.AddHttpLogging(logging => { logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode ; });
builder.Services.AddHealthChecks()
    .AddCheck<ParchmentVersionUpdaterHealthCheck>("Maven");
builder.Services.AddOpenTelemetry()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder.AddPrometheusExporter();

        metricsBuilder.AddMeter("Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel");
        metricsBuilder.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries =
                [
                    0, 0.005, 0.01, 0.025, 0.05,
                    0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                ]
            });
    });

var app = builder.Build();

app.UseHttpLogging();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

app.MapControllers();
app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint();

app.Run();
