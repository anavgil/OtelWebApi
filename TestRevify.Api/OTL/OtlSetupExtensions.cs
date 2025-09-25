using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TestRevify.Api.OTL;

public static class OtlSetupExtensions
{
    public static IServiceCollection RegisterOtl(this IServiceCollection services, string serviceName)
    {
        services.AddSingleton<Instrumentor>();
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
#if DEBUG
                            // El exportador OTLP enviará los datos al Collector
                            o.Endpoint = new Uri("http://localhost:4317");
#else                            
                            o.Endpoint = new Uri("http://otel-collector:4317");
#endif
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
#if DEBUG
                            // El exportador OTLP enviará los datos al Collector
                            o.Endpoint = new Uri("http://localhost:4317");
#else                            
                            o.Endpoint = new Uri("http://otel-collector:4317");
#endif
                        });
                });

        return services;
    }
}
