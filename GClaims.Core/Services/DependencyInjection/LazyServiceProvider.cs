using GClaims.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Core.Services.DependencyInjection;

public class LazyServiceProvider : ILazyServiceProvider, ITransientDependency
{
    public LazyServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        CachedServices = new Dictionary<Type, object>();
    }

    protected IDictionary<Type, object> CachedServices { get; set; }

    protected IServiceProvider ServiceProvider { get; set; }

    public virtual T LazyGetRequiredService<T>()
    {
        return (T)LazyGetRequiredService(typeof(T));
    }

    public virtual object LazyGetRequiredService(Type serviceType)
    {
        return CachedServices.GetOrAdd(serviceType, _ => ServiceProvider.GetRequiredService(serviceType));
    }

    public virtual T? LazyGetService<T>()
    {
        return (T)LazyGetService(typeof(T));
    }

    public virtual object? LazyGetService(Type serviceType)
    {
        return CachedServices!.GetOrAdd(serviceType, _ => ServiceProvider.GetService(serviceType));
    }

    public virtual T LazyGetService<T>(T defaultValue)
    {
        return (T)LazyGetService(typeof(T), defaultValue!);
    }

    public virtual object LazyGetService(Type serviceType, object defaultValue)
    {
        return LazyGetService(serviceType) ?? defaultValue;
    }

    public virtual T LazyGetService<T>(Func<IServiceProvider, object> factory)
    {
        return (T)LazyGetService(typeof(T), factory);
    }

    public virtual object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory)
    {
        return CachedServices.GetOrAdd(serviceType, _ => factory(ServiceProvider));
    }
}