using Microsoft.AspNetCore.HttpLogging;
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

var app = builder.Build();

app.UseHttpLogging();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
