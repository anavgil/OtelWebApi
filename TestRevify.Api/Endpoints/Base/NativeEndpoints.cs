using Microsoft.AspNetCore.Mvc;
using TestRevify.Api.Facade;
using TestRevify.Api.Workers;

namespace TestRevify.Api.Endpoints.Base;

public static class NativeEndpoints
{
    public static void RegisterNativeEndpoints(this WebApplication app)
    {
        var nativeApi = app.MapGroup("/natives");

        nativeApi.MapGet("/suma", async ([FromQuery] int val1, [FromQuery] int val2, IBackgroundTaskQueue queue, ILogger<Program> logger, CancellationToken ct) =>
        {
            await queue.QueueBackgroundWorkItemAsync(async (ct) =>
            {
                //int delayLoop = 0;
                var guid = Guid.NewGuid().ToString();
                logger.LogInformation("Queued Background Task {Guid} is starting.", guid);
                var result = await RevifyFacade.ExternalInvoke(val1, val2);
                logger.LogInformation("LLAMADA A NATIVE_CODE RESULT Suma {val1}+{val2} -> {result}", val1, val2, result);
                logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
            });
        })
        .WithDisplayName("Sum two numbers")
        .WithDescription("Call a C++ function to sum numbers")
        ;

        nativeApi.MapGet("/sleep", async ([FromQuery] int val1, IBackgroundTaskQueue queue, ILogger<Program> logger, CancellationToken ct) =>
        {
            await queue.QueueBackgroundWorkItemAsync(async (ct) =>
            {
                //int delayLoop = 0;
                var guid = Guid.NewGuid().ToString();
                logger.LogInformation("Queued Background Task {Guid} is starting.", guid);
                await RevifyFacade.ExternalSleepInvoke(val1);
                logger.LogInformation("LLAMADA A NATIVE_CODE RESULT");
                logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
            });
        })
        .WithDisplayName("Sleep some seconds")
        .WithDescription("Call a C++ function to wait a heavy process.");
    }
}
