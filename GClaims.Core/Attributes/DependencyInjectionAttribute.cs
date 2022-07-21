using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Core.Attributes;

public sealed class DependencyInjectionAttribute : Attribute
{
    public DependencyInjectionAttribute()
    {
        Lifetime = ServiceLifetime.Transient;
    }

    public DependencyInjectionAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public ServiceLifetime Lifetime { get; set; }

    public bool TryRegister { get; set; }

    public bool ReplaceServices { get; set; }

    public Type? ExposeService { get; set; }
}