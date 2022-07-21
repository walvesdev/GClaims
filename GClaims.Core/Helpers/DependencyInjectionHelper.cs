using System.Reflection;
using GClaims.Core.Attributes;
using GClaims.Core.Extensions;
using GClaims.Core.Services.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace GClaims.Core.Helpers;

public static class DependencyInjectionHelper
{
    public static void AddByConvention(IServiceCollection services, Assembly assemblie)
    {
        //var application = AppDomain.CurrentDomain.GetOrLoad("GClaims.Domain");

        var assemblies = new List<Assembly>();

        assemblies = new List<Assembly>
        {
            assemblie,
        //    application
        };

        assemblies.ForEach(assembly =>
        {
            var types = AssemblyHelper.GetAllTypes(assembly)
                .Where(type => type != null && type.IsClass && !type.IsAbstract && !type.IsGenericType).ToList();

            #region IRepository

            //if (assembly.Equals(library))
            //{
            //    var genericTypes = AssemblyHelper.GetAllTypes(assembly).Where(type => type != null && type.IsGenericType).ToList();

            //    genericTypes.ForEach(type =>
            //    {
            //        if (type.Name.Contains($"Repository"))
            //        {
            //            var attribute = type.GetCustomAttribute<DependencyInjectionAttribute>(true);

            //            var intType = type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(i => i.Name.Contains($"IRepository"));

            //            if (intType.IsNotNull() && intType.Name.Contains("Repository"))
            //            {
            //                var primaryKeyType = EntityHelper.FindPrimaryKeyType(type);
            //                if (primaryKeyType != null)
            //                {
            //                    var typeIterface = intType.MakeGenericType(type, primaryKeyType);
            //                    if (typeIterface.IsAssignableFrom(type))
            //                    {
            //                        AddType(services, type, attribute.Lifetime, typeIterface);
            //                    }
            //                }
            //            }
            //        }

            //    });
            //} 

            #endregion

            types.ForEach(type =>
            {
                if (type == null)
                {
                    return;
                }

                var attribute = type.GetCustomAttribute<DependencyInjectionAttribute>(true);

                if (attribute.IsNotNull())
                {
                    if (attribute != null)
                    {
                        AddIfHasAttribute(services, type, attribute);
                    }
                }
                else
                {
                    AddIfHasNoAttribute(services, type);
                }
            });
        });
    }

    public static void AddIfHasAttribute(IServiceCollection services, Type typeImplement,
        DependencyInjectionAttribute attribute)
    {
        var exposeService = typeImplement.GetCustomAttribute<ExposeServicesAttribute>(true);

        if (attribute.IsNotNull() && exposeService.IsNotNull())
        {
            if (attribute.ExposeService.IsNotNull())
            {
                if (typeImplement.IsAssignableTo(attribute.ExposeService))
                {
                    AddType(services, typeImplement, attribute.Lifetime, attribute.ExposeService);
                }
            }
            else
            {
                var exposeServices = exposeService?.GetExposedServiceTypes(typeImplement);

                if (exposeServices != null && !exposeServices.IsNullOrEmpty())
                {
                    exposeServices.ForEach(exServivice =>
                    {
                        if (typeImplement.IsAssignableTo(exServivice))
                        {
                            AddType(services, typeImplement, attribute.Lifetime, exServivice);
                        }
                    });
                }
            }
        }
        else if (attribute.IsNotNull() && exposeService.IsNull())
        {
            var typeIterface = GetTypeIterface(typeImplement);

            if (typeIterface.IsNotNull())
            {
                AddType(services, typeImplement, attribute.Lifetime, typeIterface);
            }
            else
            {
                AddType(services, typeImplement, attribute.Lifetime);
            }
        }
    }

    public static void AddIfHasNoAttribute(IServiceCollection services, Type type)
    {
        if (type.IsAssignableTo(typeof(ITransientDependency)))
        {
            AddDependency(services, type, ServiceLifetime.Transient);
        }

        if (type.IsAssignableTo(typeof(IScopedDependency)))
        {
            AddDependency(services, type, ServiceLifetime.Scoped);
        }

        if (type.IsAssignableTo(typeof(ISingletonDependency)))
        {
            AddDependency(services, type, ServiceLifetime.Singleton);
        }
    }

    public static void AddDependency(IServiceCollection services, Type typeImplement, ServiceLifetime serviceLifetime)
    {
        AddType(services, typeImplement, serviceLifetime, GetTypeIterface(typeImplement));
    }

    public static Type? GetTypeIterface(Type type)
    {
        return type.GetInterfaces().FirstOrDefault(i => i.Name.Equals($"I{type.Name}"));
    }

    public static void AddType(IServiceCollection services, Type typeImplement, ServiceLifetime serviceLifetime,
        Type? typeIterface = null)
    {
        if (typeIterface != null && typeIterface.IsNotNull() && typeIterface.IsAssignableFrom(typeImplement))
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeIterface, typeImplement);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeIterface, typeImplement);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeIterface, typeImplement);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }
        else
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeImplement);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeImplement);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeImplement);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }
    }
}