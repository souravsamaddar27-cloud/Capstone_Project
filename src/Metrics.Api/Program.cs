using Microsoft.EntityFrameworkCore;
using Metrics.Persistence.Db;
using Metrics.Persistence.Services;
using Metrics.Core.Interfaces;
using Metrics.Core.Enums;
using Metrics.Api.Contracts;
using Metrics.Core.Models;




var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Database
// ---------------------------
builder.Services.AddDbContext<MetricsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ---------------------------
// Application Services
// ---------------------------
builder.Services.AddScoped<IMetricsRecorder, MetricsRecorder>();
builder.Services.AddScoped<IMetricsService, MetricsService>();

// ---------------------------
// Swagger / Controllers
// ---------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


// ============================================================
// GET: Aggregated Summary
// ============================================================
//app.MapGet("/api/metrics/summary", async (
//    IMetricsService service,
//    ActivityType? activityType,
//    CancellationToken ct) =>
//{
//    var result = await service.GetSummaryAsync(activityType, ct);
//    return Results.Ok(result);
//});
app.MapGet("/api/test", async (IMetricsRecorder recorder) =>
{
    await recorder.MeasureAsync(
        ActivityType.Telemetry,
        "Test_Module",
        async () =>
        {
            await Task.Delay(200);
        });

    return Results.Ok("Recorded");
});



// ============================================================
// POST: Insert Metric
// ============================================================
app.MapPost("/api/metrics", async (
    IMetricsService service,
    CreateMetricDto dto,
    CancellationToken ct) =>
{
    var model = new CreateMetricModel
    {
        ActivityType = dto.ActivityType,
        SubActivityName = dto.SubActivityName,
        ExecutionTimeMs = dto.ExecutionTimeMs,
        MemoryBytes = dto.MemoryBytes
    };

    await service.AddMetricAsync(model, ct);

    return Results.Ok();
});


app.Run();
