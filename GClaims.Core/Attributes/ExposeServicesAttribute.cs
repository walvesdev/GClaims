using System.Reflection;
using GClaims.Core.Extensions;

namespace GClaims.Core.Attributes;

public class ExposeServicesAttribute : Attribute
{
    public ExposeServicesAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes ?? new Type[0];
    }

    public Type[] ServiceTypes { get; }

    public bool IncludeDefaults { get; set; }

    public bool IncludeSelf { get; set; }

    public List<Type> GetExposedServiceTypes(Type targetType)
    {
        var serviceList = ServiceTypes.ToList();

        if (IncludeDefaults)
        {
            foreach (var type in GetDefaultServices(targetType))
            {
                serviceList.AddIfNotContains(type);
            }

            if (IncludeSelf)
            {
                serviceList.AddIfNotContains(targetType);
            }
        }
        else if (IncludeSelf)
        {
            serviceList.AddIfNotContains(targetType);
        }

        return serviceList;
    }

    private static List<Type> GetDefaultServices(Type type)
    {
        var serviceTypes = new List<Type>();

        foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
        {
            var interfaceName = interfaceType.Name;

            if (interfaceName.StartsWith("I"))
            {
                interfaceName = interfaceName.Right(interfaceName.Length - 1);
            }

            if (type.Name.EndsWith(interfaceName))
            {
                serviceTypes.Add(interfaceType);
            }
        }

        return serviceTypes;
    }
}