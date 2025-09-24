using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;
using TestRevify.Api.Endpoints;
using TestRevify.Api.Endpoints.Base;
using TestRevify.Api.Workers;
using TestRevify.Api.OTL;
using Serilog.Enrichers.Span;

var serviceName = "WebApi";
var builder = WebApplication.CreateSlimBuilder(args);
const string outputTemplate =
    "[{Level:w}]: {Timestamp:dd-MM-yyyy:HH:mm:ss} {MachineName} {EnvironmentName} {SourceContext} {Message}{NewLine}{Exception}";

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .ReadFrom.Services(services)
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        // El exportador OTLP enviará los datos al Collector
        //options.Endpoint = "http://otel-collector:4317";
        options.Endpoint = "http://localhost:4317";
        options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName,
            ["deployment.environment"] = "production"
        };
    })
    );

builder.Services.RegisterOtl(serviceName);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
{
    if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
        queueCapacity = 100;
    return new BackgroundTaskQueue(queueCapacity);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.RegisterTodoItemsEndpoints();
app.RegisterNativeEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
    .WithTheme(ScalarTheme.Kepler)
    .WithLayout(ScalarLayout.Modern)
    .WithFavicon("https://scalar.com/logo-light.svg");
});

app.UseHttpsRedirection();

app.Run();

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
