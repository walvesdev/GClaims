using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para todos os objetos.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Usado para simplificar e embelezar a conversão de um objeto para um tipo.
    /// </summary>
    /// <typeparam name="T">Tipo a ser convertido</typeparam>
    /// <param name="obj">Objeto a ser lançado</param>
    /// <returns>Objeto convertido</returns>
    public static T As<T>(this object obj) where T : class
    {
        return (T)obj;
    }

    /// <summary>
    /// Converte determinado objeto em um tipo de valor usando o método
    /// <see cref="M:System.Convert.ChangeType(System.Object,System.Type)" />.
    /// </summary>
    /// <param name="obj">Objeto a ser convertido</param>
    /// <typeparam name="T">Tipo do objeto alvo</typeparam>
    /// <returns>Objeto convertido</returns>
    public static T To<T>(this object obj) where T : struct
    {
        if (typeof(T) == typeof(Guid))
        {
            return (T)TypeDescriptor.GetConverter(typeof(T))
                .ConvertFromInvariantString(obj.ToString() ?? string.Empty)!;
        }

        return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Verifica se um item está em uma lista.
    /// </summary>
    /// <param name="item">Item a verificar</param>
    /// <param name="list">Lista de itens</param>
    /// <typeparam name="T">Tipo dos itens</typeparam>
    public static bool IsIn<T>(this T item, params T[] list)
    {
        return list.Contains(item);
    }

    /// <summary>
    /// Verifica se um item está no enumerável fornecido.
    /// </summary>
    /// <param name="item">Item a verificar</param>
    /// <param name="items">Itens</param>
    /// <typeparam name="T">Tipo dos itens</typeparam>
    public static bool IsIn<T>(this T item, IEnumerable<T> items)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Pode ser usado para executar condicionalmente uma função
    /// em um objeto e returna o objeto modificado ou original.
    /// É útil para chamadas encadeadas.
    /// </summary>
    /// <param name="obj">Um objeto</param>
    /// <param name="condition">Uma condição</param>
    /// <param name="func">Uma função que é executada somente se a condição for <code>true</code></param>
    /// <typeparam name="T">Tipo do objeto</typeparam>
    /// <returns>
    /// returna o objeto modificado (pelo <paramref name="func" /> se o <paramref name="condition" /> for <code>true</code>)
    /// ou o objeto original se o <paramref name="condition" /> for <code>false</code>
    /// </returns>
    public static T If<T>(this T obj, bool condition, Func<T, T> func)
    {
        if (condition)
        {
            return func(obj);
        }

        return obj;
    }

    /// <summary>
    /// Pode ser usado para realizar uma ação condicionalmente
    /// em um objeto e retorna o objeto original.
    /// É útil para chamadas encadeadas no objeto.
    /// </summary>
    /// <param name="obj">Um objeto</param>
    /// <param name="condition">Uma condição</param>
    /// <param name="action">Uma ação que é executada somente se a condição for <code>true</code></param>
    /// <typeparam name="T">Tipo do objeto</typeparam>
    /// <returns>
    /// Retorna o objeto original.
    /// </returns>
    public static T If<T>(this T obj, bool condition, Action<T> action)
    {
        if (condition)
        {
            action(obj);
        }

        return obj;
    }

    public static bool IsNotNull<T>(this T item)
    {
        return item != null;
    }

    public static bool IsNull<T>(this T item)
    {
        return item == null;
    }

    public static string? ToJson(object data)
    {
        return data == null ? null : JsonConvert.SerializeObject(data);
    }

    public static void BindObjectValue<T>(this T target, T source) where T : class, new()
    {
        var instanceTarget = Activator.CreateInstance(typeof(T)) as T;
        var instanceSource = Activator.CreateInstance(typeof(T)) as T;

        instanceTarget = target;
        instanceSource = source;

        foreach (var propertyTarget in target.GetType().GetProperties())
        {
            foreach (var propertySource in source.GetType().GetProperties())
            {
                if (propertyTarget.Name != propertySource.Name)
                {
                    continue;
                }

                var value = propertySource.GetValue(instanceSource, null);
                propertyTarget.SetValue(instanceTarget, value, null);
            }
        }

        instanceTarget = null;
        instanceSource = null;
    }

    public static void BindObjectValue<T, TProperty>(this T target, T source,
        Expression<Func<T, TProperty>>? ignoreProperty = null) where T : class, new()
    {
        var instanceTarget = Activator.CreateInstance(typeof(T)) as T;
        var instanceSource = Activator.CreateInstance(typeof(T)) as T;

        instanceTarget = target;
        instanceSource = source;

        if (ignoreProperty != null)
        {
            var name = GetPropertyName(ignoreProperty);

            foreach (var propertyTarget in target.GetType().GetProperties())
            {
                foreach (var propertySource in source.GetType().GetProperties())
                {
                    if (propertyTarget.Name == propertySource.Name && propertyTarget.Name != name)
                    {
                        var value = propertySource.GetValue(instanceSource, null);
                        propertyTarget.SetValue(instanceTarget, value, null);
                    }
                }
            }
        }

        instanceTarget = null;
        instanceSource = null;
    }

    public static void BindObjectValue<T>(this T target, T source, string[]? propertiesName = null)
        where T : class, new()
    {
        var instanceTarget = Activator.CreateInstance(typeof(T)) as T;
        var instanceSource = Activator.CreateInstance(typeof(T)) as T;

        instanceTarget = target;
        instanceSource = source;

        foreach (var propertyTarget in target.GetType().GetProperties())
        {
            foreach (var propertySource in source.GetType().GetProperties())
            {
                if (propertiesName != null && (propertyTarget.Name != propertySource.Name ||
                                               CheckIfExists(propertiesName, propertyTarget.Name)))
                {
                    continue;
                }

                var value = propertySource.GetValue(instanceSource, null);
                propertyTarget.SetValue(instanceTarget, value, null);
            }
        }

        instanceTarget = null;
        instanceSource = null;
    }

    private static bool CheckIfExists(IEnumerable<string> propertiesName, string propName)
    {
        var exists = false;
        foreach (var name in propertiesName)
        {
            exists = name == propName;
        }

        return exists;
    }

    private static string GetPropertyName<TSource, TField>(Expression<Func<TSource, TField>> field)
    {
        if (Equals(field, null))
        {
            throw new NullReferenceException("Field is required");
        }

        MemberExpression expr = null;

        switch (field.Body)
        {
            case MemberExpression body:
                expr = body;
                break;
            case UnaryExpression expression:
                expr = (MemberExpression)expression.Operand;
                break;
            default:
            {
                const string Format = "Expression '{0}' not supported.";
                var message = string.Format(Format, field);

                throw new ArgumentException(message, nameof(field));
            }
        }

        return expr.Member.Name;
    }

    public static object? GetPropertyValue(this object src, string propName)
    {
        return src.GetType().GetProperty(propName)?.GetValue(src, null);
    }

    public static string? Serialize(object data)
    {
        return data is not null ? JsonConvert.SerializeObject(data) : default;
    }

    public static T? Deserialize<T>(string? data)
    {
        return data is not null ? JsonConvert.DeserializeObject<T>(data) : default;
    }

    public static TTarget Assign<TSource, TTarget>(this TTarget? target, TSource source)
        where TSource : class
        where TTarget : class
    {
        var obj = Serialize(source);
        return target = Deserialize<TTarget>(obj!)!;
    }

    public static TTarget? Map<TSource, TTarget>(this TSource source)
        where TSource : class, new()
        where TTarget : class, new()
    {
        var target = new TTarget();
        var obj = Serialize(source);
        target = Deserialize<TTarget>(obj!)!;
        return target;
    }

    public static object? GetId(this object src)
    {
        return src.GetType().GetProperty("Id")?.GetValue(src, null);
    }

    public static bool CompareId<T>(this T src, T obj)
        where T : class, new()
    {
        if (src == null || obj == null)
        {
            return false;
        }

        var idSrc = src.GetType().GetProperty("Id")?.GetValue(src, null)!;
        var idObj = src.GetType().GetProperty("Id")?.GetValue(obj, null)!;

        if (idSrc == null || idObj == null)
        {
            return false;
        }

        if (idSrc.GetType() != idObj.GetType())
        {
            return false;
        }

        return idSrc == idObj;
    }
}