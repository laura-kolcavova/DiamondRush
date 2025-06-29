namespace DiamondRush.MonoGame.Core.Services;

public static class ServiceProviderExtensions
{
    public static TService? GetService<TService>(
        this IServiceProvider serviceProvider)
        where TService : class
    {
        return serviceProvider.GetService(typeof(TService)) as TService;
    }

    public static TService GetRequiredService<TService>(
        this IServiceProvider serviceProvider)
        where TService : class
    {
        var service = serviceProvider.GetService(typeof(TService)) as TService;

        return service
            ?? throw new InvalidOperationException($"Service of type {typeof(TService).FullName} is not registered.");
    }
}
