using System.Reflection;
using GClaims.Core.Helpers;

namespace GClaims.Core.Extensions;

public static class AppDomainExtensions
{
    public static Assembly GetOrLoad(this AppDomain appDomain, string assemblyName)
    {
        Check.NotNullOrWhiteSpace(assemblyName, "O nome do assembly deve ser fornecido");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
        {
            var name = a.GetName().Name;
            return name != null && name.Contains("GClaims");
        }).ToList();

        if (assemblies.IsNullOrEmpty())
        {
            return AppDomain.CurrentDomain.Load(assemblyName);
        }

        {
            var assembly = assemblies.FirstOrDefault(a => a.GetName().Name!.Equals(assemblyName));

            if (assembly.IsNotNull())
            {
                return assembly!;
            }
        }

        return AppDomain.CurrentDomain.Load(assemblyName);
    }
}