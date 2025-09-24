using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TestRevify.Api.OTL;

public static class OtlSetupExtensions
{
    public static IServiceCollection RegisterOtl(this IServiceCollection services, string serviceName)
    {
        services.AddSingleton<Instrumentor>();

        //services.AddOpenTelemetry()
        //    .WithTracing(tracing => tracing
        //        .AddSource(Instrumentor.ServiceName)
        //        .ConfigureResource(resource => resource
        //            .AddService(Instrumentor.ServiceName))
        //        .AddAspNetCoreInstrumentation()
        //        .AddHttpClientInstrumentation()
        //        .AddOtlpExporter(options =>
        //        {
        //            options.Endpoint = new Uri("http://localhost:")
        //        }))
        //    .WithMetrics(metrics => metrics
        //        .AddMeter(Instrumentor.ServiceName)
        //        .ConfigureResource(resource => resource
        //            .AddService(Instrumentor.ServiceName))
        //        .AddRuntimeInstrumentation()
        //        .AddAspNetCoreInstrumentation()
        //        //.AddProcessInstrumentation()
        //        .AddHttpClientInstrumentation()
        //        //.AddEventCountersInstrumentation(c =>
        //        //{
        //        //    // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
        //        //    c.AddEventSources(
        //        //        "Microsoft.AspNetCore.Hosting",
        //        //        "Microsoft-AspNetCore-Server-Kestrel",
        //        //        "System.Net.Http",
        //        //        "System.Net.Sockets");
        //        //})
        //        .AddOtlpExporter());

        services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource(serviceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                               .AddService(serviceName: serviceName))
            .AddAspNetCoreInstrumentation() // Rastrea peticiones entrantes
            .AddHttpClientInstrumentation() // Rastrea llamadas salientes
            .AddSource("MyApplicationActivitySource")
            .AddOtlpExporter(o =>
            {
                // El exportador OTLP enviará los datos al Collector
                //o.Endpoint = new Uri("http://otel-collector:4317");
                o.Endpoint = new Uri("http://localhost:4317");
            });
    })
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter(o =>
            {
                //o.Endpoint = new Uri("http://otel-collector:4317");
                o.Endpoint = new Uri("http://localhost:4317");
            });
    })
    ;

        return services;
    }
}
