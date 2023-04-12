using FluentValidation;
using FolkLibrary;
using FolkLibrary.Behaviors;
using FolkLibrary.Interfaces;
using FolkLibrary.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var currentAsm = typeof(DependencyInjection).Assembly;
        var callingAsm = Assembly.GetCallingAssembly();
        services.AddSingleton<IEncryptorProvider, EncryptorProvider>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddAutoMapper(config => config.AddProfile(new AutoMapperProfile(currentAsm, callingAsm)));
        services.AddMediatR(opts => opts.RegisterServicesFromAssemblies(currentAsm, callingAsm));
        services.AddValidatorsFromAssembly(currentAsm);
        services.AddValidatorsFromAssembly(callingAsm);
        services.AddScoped(typeof(IValidatorService<>), typeof(ValidatorService<>));
        return services;
    }

    public static IServiceCollection AddFolkHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IFolkHttpClient, FolkHttpClient>(opts => opts.BaseAddress = new(configuration.GetConnectionString("LetsTalk.WebApi")!));
        return services;
    }
}
