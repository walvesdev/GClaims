﻿using System.Reflection;
using System.Runtime.Loader;

namespace GClaims.Core.Helpers;

public static class AssemblyHelper
{
    public static List<Assembly> LoadAssemblies(string folderPath, SearchOption searchOption)
    {
        return GetAssemblyFiles(folderPath, searchOption)
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            .ToList();
    }

    public static IEnumerable<string> GetAssemblyFiles(string folderPath, SearchOption searchOption)
    {
        return Directory
            .EnumerateFiles(folderPath, "*.*", searchOption)
            .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
    }

    public static IEnumerable<Type?> GetAllTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types;
        }
    }
}