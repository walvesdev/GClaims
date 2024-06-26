﻿using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using GClaims.Core.Extensions;

namespace GClaims.Core.Helpers;

public static class TypeHelper
{
    private static readonly HashSet<Type> FloatingTypes = new HashSet<Type>
    {
        typeof(float),
        typeof(double),
        typeof(decimal)
    };

    private static readonly HashSet<Type> NonNullablePrimitiveTypes = new HashSet<Type>
    {
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(sbyte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(bool),
        typeof(float),
        typeof(decimal),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    };

    public static bool IsNonNullablePrimitiveType(Type type)
    {
        return NonNullablePrimitiveTypes.Contains(type);
    }

    public static bool IsFunc(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        var type = obj.GetType();
        if (!type.GetTypeInfo().IsGenericType)
        {
            return false;
        }

        return type.GetGenericTypeDefinition() == typeof(Func<>);
    }

    public static bool IsFunc<TReturn>(object obj)
    {
        if (obj != null)
        {
            return obj.GetType() == typeof(Func<TReturn>);
        }

        return false;
    }

    public static bool IsPrimitiveExtended(Type type, bool includeNullables = true, bool includeEnums = false)
    {
        if (IsPrimitiveExtendedInternal(type, includeEnums))
        {
            return true;
        }

        if (includeNullables && IsNullable(type) && type.GenericTypeArguments.Any())
        {
            return IsPrimitiveExtendedInternal(type.GenericTypeArguments[0], includeEnums);
        }

        return false;
    }

    public static bool IsNullable(Type type)
    {
        if (type.IsGenericType)
        {
            return type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        return false;
    }

    public static Type? GetFirstGenericArgumentIfNullable(this Type? t)
    {
        if (t != null && t.GetGenericArguments().Length != 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return t.GetGenericArguments().FirstOrDefault();
        }

        return t;
    }

    public static bool IsEnumerable(Type type, out Type? itemType, bool includePrimitives = true)
    {
        if (!includePrimitives && IsPrimitiveExtended(type))
        {
            itemType = null;
            return false;
        }

        var enumerableTypes = ReflectionHelper.GetImplementedGenericTypes(type, typeof(IEnumerable<>));
        if (enumerableTypes.Count == 1)
        {
            itemType = enumerableTypes[0].GenericTypeArguments[0];
            return true;
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            itemType = typeof(object);
            return true;
        }

        itemType = null;
        return false;
    }

    public static bool IsDictionary(Type type, out Type? keyType, out Type? valueType)
    {
        var dictionaryTypes = ReflectionHelper.GetImplementedGenericTypes(type, typeof(IDictionary<,>));
        if (dictionaryTypes.Count == 1)
        {
            keyType = dictionaryTypes[0].GenericTypeArguments[0];
            valueType = dictionaryTypes[0].GenericTypeArguments[1];
            return true;
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            keyType = typeof(object);
            valueType = typeof(object);
            return true;
        }

        keyType = null;
        valueType = null;
        return false;
    }

    private static bool IsPrimitiveExtendedInternal(Type type, bool includeEnums)
    {
        if (type.IsPrimitive)
        {
            return true;
        }

        if (includeEnums && type.IsEnum)
        {
            return true;
        }

        if (!(type == typeof(string)) && !(type == typeof(decimal)) && !(type == typeof(DateTime)) &&
            !(type == typeof(DateTimeOffset)) && !(type == typeof(TimeSpan)))
        {
            return type == typeof(Guid);
        }

        return true;
    }

    public static T? GetDefaultValue<T>()
    {
        return default;
    }

    public static object? GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    public static string GetFullNameHandlingNullableAndGenerics(Type type)
    {
        Check.NotNull(type, "type");
        switch (type.IsGenericType)
        {
            case true when type.GetGenericTypeDefinition() == typeof(Nullable<>):
                return type.GenericTypeArguments[0].FullName + "?";
            case false:
                return type.FullName ?? type.Name;
        }

        var genericType = type.GetGenericTypeDefinition();
        if (genericType.FullName != null)
        {
            return genericType.FullName.Left(genericType.FullName!.IndexOf('`')) + "<" + type.GenericTypeArguments
                .Select(GetFullNameHandlingNullableAndGenerics).JoinAsString(",") + ">";
        }

        return type.FullName ?? type.Name;
    }

    public static string GetSimplifiedName(Type type)
    {
        Check.NotNull(type, "type");
        switch (type.IsGenericType)
        {
            case true when type.GetGenericTypeDefinition() == typeof(Nullable<>):
                return GetSimplifiedName(type.GenericTypeArguments[0]) + "?";
            case true:
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType.FullName != null)
                {
                    return genericType.FullName.Left(genericType.FullName!.IndexOf('`')) + "<" +
                           type.GenericTypeArguments.Select(GetSimplifiedName).JoinAsString(",") + ">";
                }

                break;
            }
        }

        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(int))
        {
            return "number";
        }

        if (type == typeof(long))
        {
            return "number";
        }

        if (type == typeof(bool))
        {
            return "boolean";
        }

        if (type == typeof(char))
        {
            return "string";
        }

        if (type == typeof(double))
        {
            return "number";
        }

        if (type == typeof(float))
        {
            return "number";
        }

        if (type == typeof(decimal))
        {
            return "number";
        }

        if (type == typeof(DateTime))
        {
            return "string";
        }

        if (type == typeof(DateTimeOffset))
        {
            return "string";
        }

        if (type == typeof(TimeSpan))
        {
            return "string";
        }

        if (type == typeof(Guid))
        {
            return "string";
        }

        if (type == typeof(byte))
        {
            return "number";
        }

        if (type == typeof(sbyte))
        {
            return "number";
        }

        if (type == typeof(short))
        {
            return "number";
        }

        if (type == typeof(ushort))
        {
            return "number";
        }

        if (type == typeof(uint))
        {
            return "number";
        }

        if (type == typeof(ulong))
        {
            return "number";
        }

        if (type == typeof(IntPtr))
        {
            return "number";
        }

        if (type == typeof(UIntPtr))
        {
            return "number";
        }

        if (type == typeof(object))
        {
            return "object";
        }

        return type.FullName ?? type.Name;
    }

    public static object? ConvertFromString<TTargetType>(string value)
    {
        return ConvertFromString(typeof(TTargetType), value);
    }

    public static object? ConvertFromString(Type targetType, string value)
    {
        if (value == null)
        {
            return null;
        }

        var converter = TypeDescriptor.GetConverter(targetType);
        if (!IsFloatingType(targetType))
        {
            return converter.ConvertFromString(value);
        }

        using (CultureHelper.Use(CultureInfo.InvariantCulture))
        {
            return converter.ConvertFromString(value.Replace(',', '.'));
        }
    }

    public static bool IsFloatingType(Type type, bool includeNullable = true)
    {
        if (FloatingTypes.Contains(type))
        {
            return true;
        }

        return includeNullable && IsNullable(type) && FloatingTypes.Contains(type.GenericTypeArguments[0]);
    }

    public static object? ConvertFrom<TTargetType>(object value)
    {
        return ConvertFrom(typeof(TTargetType), value);
    }

    public static object? ConvertFrom(Type targetType, object value)
    {
        return TypeDescriptor.GetConverter(targetType).ConvertFrom(value);
    }

    public static Type StripNullable(Type type)
    {
        if (!IsNullable(type))
        {
            return type;
        }

        return type.GenericTypeArguments[0];
    }

    public static bool IsDefaultValue(object obj)
    {
        return obj?.Equals(GetDefaultValue(obj.GetType())) ?? true;
    }
}