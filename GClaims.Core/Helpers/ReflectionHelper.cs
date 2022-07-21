using System.Reflection;
using GClaims.Core.Extensions;

namespace GClaims.Core.Helpers;

public static class ReflectionHelper
{
    /// <summary>
    /// Verifica se <paramref name="givenType" /> implementa/herda <paramref name="genericType" />.
    /// </summary>
    /// <param name="givenType">Digite para verificar</param>
    /// <param name="genericType">Tipo genérico</param>
    public static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        var givenTypeInfo = givenType.GetTypeInfo();
        if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        var interfaces = givenTypeInfo.GetInterfaces();
        foreach (var interfaceType in interfaces)
        {
            if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }
        }

        if (givenTypeInfo.BaseType == null)
        {
            return false;
        }

        return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
    }

    public static List<Type> GetImplementedGenericTypes(Type givenType, Type genericType)
    {
        var result = new List<Type>();
        AddImplementedGenericTypes(result, givenType, genericType);
        return result;
    }

    private static void AddImplementedGenericTypes(List<Type> result, Type givenType, Type genericType)
    {
        var givenTypeInfo = givenType.GetTypeInfo();
        if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            result.AddIfNotContains(givenType);
        }

        var interfaces = givenTypeInfo.GetInterfaces();
        foreach (var interfaceType in interfaces)
        {
            if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
            {
                result.AddIfNotContains(interfaceType);
            }
        }

        if (!(givenTypeInfo.BaseType == null))
        {
            AddImplementedGenericTypes(result, givenTypeInfo.BaseType, genericType);
        }
    }

    /// <summary>
    /// Tenta obter um atributo de definido para um membro de classe e está declarando o tipo incluindo atributos herdados.
    /// Retorna o valor padrão se não for declarado.
    /// </summary>
    /// <typeparam name="TAttribute">Tipo do atributo</typeparam>
    /// <param name="memberInfo">MemberInfo</param>
    /// <param name="defaultValue">Valor padrão (nulo como padrão)</param>
    /// <param name="inherit">Herdar atributo das classes base</param>
    public static TAttribute? GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo,
        TAttribute? defaultValue = null, bool inherit = true) where TAttribute : Attribute
    {
        if (memberInfo.IsDefined(typeof(TAttribute), inherit))
        {
            return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
        }

        return defaultValue;
    }

    /// <summary>
    /// Tenta obter um atributo de definido para um membro de classe e está declarando o tipo incluindo atributos herdados.
    /// Retorna o valor padrão se não for declarado.
    /// </summary>
    /// <typeparam name="TAttribute">Tipo do atributo</typeparam>
    /// <param name="memberInfo">MemberInfo</param>
    /// <param name="defaultValue">Valor padrão (nulo como padrão)</param>
    /// <param name="inherit">Herdar atributo das classes base</param>
    public static TAttribute? GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo,
        TAttribute? defaultValue = null, bool inherit = true) where TAttribute : class
    {
        object obj = memberInfo.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault();
        if (obj == null)
        {
            var declaringType = memberInfo.DeclaringType;
            obj = ((object)declaringType != null
                ? declaringType.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>()
                    .FirstOrDefault()
                : null) ?? defaultValue;
        }

        return (TAttribute)obj;
    }

    /// <summary>
    /// Tenta obter os atributos definidos para um membro da classe e está declarando o tipo incluindo os atributos herdados.
    /// </summary>
    /// <typeparam name="TAttribute">Tipo do atributo</typeparam>
    /// <param name="memberInfo">MemberInfo</param>
    /// <param name="inherit">Herdar atributo das classes base</param>
    public static IEnumerable<TAttribute> GetAttributesOfMemberOrDeclaringType<TAttribute>(MemberInfo memberInfo,
        bool inherit = true) where TAttribute : class
    {
        var customAttributes = memberInfo.GetCustomAttributes(true).OfType<TAttribute>();
        var declaringTypeCustomAttributes =
            memberInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>();
        if (declaringTypeCustomAttributes == null)
        {
            return customAttributes;
        }

        return customAttributes.Concat(declaringTypeCustomAttributes).Distinct();
    }

    /// <summary>
    /// Obtém o valor de uma propriedade por seu caminho completo de determinado objeto
    /// </summary>
    public static object? GetValueByPath(object obj, Type objectType, string propertyPath)
    {
        var value = obj;
        var currentType = objectType;
        var objectPath = currentType.FullName;
        var absolutePropertyPath = propertyPath;
        if (objectPath != null && absolutePropertyPath.StartsWith(objectPath))
        {
            absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
        }

        var array = absolutePropertyPath.Split(new[] { '.' });
        foreach (var propertyName in array)
        {
            var property = currentType.GetProperty(propertyName);
            if (property != null)
            {
                if (value != null)
                {
                    value = property.GetValue(value, null);
                }

                currentType = property.PropertyType;
                continue;
            }

            value = null;
            break;
        }

        return value;
    }

    /// <summary>
    /// Define o valor de uma propriedade por seu caminho completo em determinado objeto
    /// </summary>
    internal static void SetValueByPath(object obj, Type objectType, string propertyPath, object value)
    {
        var currentType = objectType;
        var objectPath = currentType.FullName;
        var absolutePropertyPath = propertyPath;
        if (objectPath != null && absolutePropertyPath.StartsWith(objectPath))
        {
            absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
        }

        var properties = absolutePropertyPath.Split(new[] { '.' });
        if (properties.Length == 1)
        {
            objectType.GetProperty(properties.First())!.SetValue(obj, value);
            return;
        }

        for (var i = 0; i < properties.Length - 1; i++)
        {
            var property = currentType.GetProperty(properties[i]);
            obj = property!.GetValue(obj, null);
            currentType = property!.PropertyType;
        }

        currentType.GetProperty(properties.Last())!.SetValue(obj, value);
    }

    /// <summary>
    /// Obtém todos os valores constantes no tipo especificado (incluindo o tipo base).
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string[] GetPublicConstantsRecursively(Type type)
    {
        var list = new List<string>();
        Recursively(list, type, 1);
        return list.ToArray();

        static void Recursively(List<string> constants, Type targetType, int currentDepth)
        {
            if (currentDepth <= 8)
            {
                constants.AddRange(
                    from x in targetType.GetFields(BindingFlags.Static | BindingFlags.Public |
                                                   BindingFlags.FlattenHierarchy)
                    where x.IsLiteral && !x.IsInitOnly
                    select x.GetValue(null)!.ToString());
                var nestedTypes = targetType.GetNestedTypes(BindingFlags.Public);
                foreach (var nestedType in nestedTypes)
                {
                    Recursively(constants, nestedType, currentDepth + 1);
                }
            }
        }
    }
}