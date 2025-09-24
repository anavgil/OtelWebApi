using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TestRevify.Api.Endpoints.Base;

namespace TestRevify.Api.Extensions;

public static class RegisterEndpointExtension
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = [.. assembly
                                                .DefinedTypes
                                                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                                                               type.IsAssignableTo(typeof(IEndpoint)))
                                                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))];

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="app"></param>
    /// <param name="routeGroupBuilder"></param>
    /// <returns></returns>
    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services
                                            .GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}
