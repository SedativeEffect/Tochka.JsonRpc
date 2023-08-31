﻿using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Asp.Versioning.ApplicationModels;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Tochka.JsonRpc.Server.Binding;
using Tochka.JsonRpc.Server.DependencyInjection;
using Tochka.JsonRpc.Server.Filters;
using Tochka.JsonRpc.Server.Routing;
using Tochka.JsonRpc.Server.Services;
using Tochka.JsonRpc.Server.Settings;

namespace Tochka.JsonRpc.Server.Extensions;

/// <summary>
/// Extensions to configure JSON-RPC API
/// </summary>
[PublicAPI]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Register services required for JSON-RPC calls processing and configure server options
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
    /// <param name="configureOptions">Delegate used to configure server options</param>
    public static IServiceCollection AddJsonRpcServer(this IServiceCollection services, Action<JsonRpcServerOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.TryAddConvention<JsonRpcActionModelConvention>();
        services.TryAddConvention<JsonRpcParameterModelConvention>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, JsonRpcMatcherPolicy>());
        services.AddSingleton<IJsonRpcParamsParser, JsonRpcParamsParser>();
        services.AddSingleton<IJsonRpcParameterBinder, JsonRpcParameterBinder>();
        services.AddSingleton<IJsonRpcRequestHandler, JsonRpcRequestHandler>();
        services.AddSingleton<IJsonRpcExceptionWrapper, JsonRpcExceptionWrapper>();
        services.AddSingleton<IJsonRpcRequestValidator, JsonRpcRequestValidator>();
        services.Configure<MvcOptions>(static options =>
        {
            options.Filters.Add<JsonRpcActionFilter>(int.MaxValue);
            options.Filters.Add<JsonRpcExceptionFilter>(int.MaxValue);
            options.Filters.Add<JsonRpcResultFilter>(int.MaxValue);
        });
        services.AddSingleton<IJsonRpcErrorFactory, JsonRpcErrorFactory>();
        services.AddApiVersioning(static options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
            })
            .AddMvc()
            .AddApiExplorer(static options =>
            {
                options.SubstituteApiVersionInUrl = true;
                options.GroupNameFormat = "'v'VVV";
                options.FormatGroupName = static (name, version) => $"{name}_{version}";
            });
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IApiControllerSpecification, JsonRpcControllerSpecification>());
        services.AddSingleton<JsonRpcMarkerService>();
        return services;
    }

    /// <summary>
    /// Register services required for JSON-RPC calls processing
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to</param>
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddJsonRpcServer(this IServiceCollection services) => services.AddJsonRpcServer(static _ => { });

    /// <summary>
    /// Use middleware to process JSON-RPC calls
    /// </summary>
    /// <param name="app">Application to add middleware to</param>
    /// <exception cref="InvalidOperationException">If AddJsonRpcServer was not called before</exception>
    [ExcludeFromCodeCoverage(Justification = "it's almost impossible to test UseMiddleware")]
    public static IApplicationBuilder UseJsonRpc(this IApplicationBuilder app)
    {
        EnsureRequiredServicesRegistered(app.ApplicationServices);
        // Unfortunately there is no good way to check if UseRouting was called before it
        return app.UseMiddleware<JsonRpcMiddleware>();
    }

    private static IServiceCollection TryAddConvention<T>(this IServiceCollection serviceCollection)
        where T : class
    {
        serviceCollection.TryAddSingleton<T>();
        serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(IConfigureOptions<MvcOptions>), typeof(ModelConventionConfigurator<T>), ServiceLifetime.Singleton));
        return serviceCollection;
    }

    private static void EnsureRequiredServicesRegistered(IServiceProvider services)
    {
        if (services.GetService<JsonRpcMarkerService>() == null)
        {
            throw new InvalidOperationException($"Unable to find the required services. Please add all the required services by calling '{nameof(IServiceCollection)}.{nameof(AddJsonRpcServer)}' in the application startup code.");
        }
    }
}