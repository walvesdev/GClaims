using GClaims.Core.Helpers;

namespace GClaims.Core.Extensions;

public static class TypeExtensions
{
    public static string GetFullNameWithAssemblyName(this Type type)
    {
        return type.FullName + ", " + type.Assembly.GetName().Name;
    }

    /// <summary>
    /// Determina se uma instância deste tipo pode ser atribuída a
    /// uma instância do <typeparamref name="TTarget"></typeparamref>.
    /// Usa internamente <see cref="M:System.Type.IsAssignableFrom(System.Type)" />.
    /// </summary>
    /// <typeparam name="TTarget">Tipo de destino</typeparam>
    /// (como reverso).
    public static bool IsAssignableTo<TTarget>(this Type type)
    {
        Check.NotNull(type, "type");
        return type.IsAssignableTo(typeof(TTarget));
    }

    /// <summary>
    /// Determina se uma instância deste tipo pode ser atribuída a
    /// uma instância do <paramref name="targetType"></paramref>.
    /// Usa internamente <see cref="M:System.Type.IsAssignableFrom(System.Type)" /> (como reverso).
    /// </summary>
    /// <param name="type">este tipo</param>
    /// <param name="targetType">Tipo de destino</param>
    public static bool IsAssignableTo(this Type type, Type targetType)
    {
        Check.NotNull(type, "type");
        Check.NotNull(targetType, "targetType");
        return targetType.IsAssignableFrom(type);
    }

    /// <summary>
    /// Obtém todas as classes base deste tipo.
    /// </summary>
    /// <param name="type">O tipo para obter suas classes base.</param>
    /// <param name="includeObject">Verdadeiro, para incluir o tipo padrão <see cref="T:System.Object" /> no array retornado.</param>
    public static Type[] GetBaseClasses(this Type type, bool includeObject = true)
    {
        Check.NotNull(type, "type");
        var list = new List<Type>();
        if (type.BaseType != null)
        {
            AddTypeAndBaseTypesRecursively(list, type.BaseType, includeObject);
        }

        return list.ToArray();
    }

    /// <summary>
    /// Obtém todas as classes base deste tipo.
    /// </summary>
    /// <param name="type">O tipo para obter suas classes base.</param>
    /// <param name="stoppingType">
    /// Um tipo para parar de ir para as classes base mais profundas. Este tipo será incluído no
    /// array retornado
    /// </param>
    /// <param name="includeObject">Verdadeiro, para incluir o tipo padrão <see cref="T:System.Object" /> no array retornado.</param>
    public static Type[] GetBaseClasses(this Type type, Type stoppingType, bool includeObject = true)
    {
        Check.NotNull(type, "type");
        var list = new List<Type>();
        if (type.BaseType != null)
        {
            AddTypeAndBaseTypesRecursively(list, type.BaseType, includeObject, stoppingType);
        }

        return list.ToArray();
    }

    public static object?[] GetKeys(this Type type)
    {
        var key = type.GetProperty($"{type.Name}Id");
        var id = Convert.ChangeType(key, key?.PropertyType!);

        return id.IsNotNull() ? new[] { id } : Array.Empty<object>();
    }

    public static string GetTypeName(this Type type)
    {
        var instance = Activator.CreateInstance(type);
        var typeName = instance?.GetType().Name;
        return typeName!;
    }

    private static void AddTypeAndBaseTypesRecursively(ICollection<Type> types, Type type, bool includeObject,
        Type? stoppingType = null)
    {
        if (type == null || type == stoppingType || (!includeObject && type == typeof(object)))
        {
            return;
        }

        AddTypeAndBaseTypesRecursively(types, type.BaseType!, includeObject, stoppingType);
        types.Add(type);
    }
}