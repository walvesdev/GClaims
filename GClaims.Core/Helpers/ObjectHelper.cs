using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using GClaims.Core.Extensions;
using Newtonsoft.Json;

namespace GClaims.Core.Helpers;

public static class ObjectHelper
{
    private static readonly ConcurrentDictionary<string, PropertyInfo> CachedObjectProperties =
        new ConcurrentDictionary<string, PropertyInfo>();

    public static string SerializeObject<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static object DeserializeObject(this string obj)
    {
        return JsonConvert.DeserializeObject(obj);
    }

    public static T DeserializeObject<T>(this string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }

    public static void TrySetProperty<TObject, TValue>(TObject obj, Expression<Func<TObject, TValue>> propertySelector,
        Func<TValue> valueFactory, params Type[] ignoreAttributeTypes)
    {
        TrySetProperty(obj, propertySelector, _ => valueFactory(), ignoreAttributeTypes);
    }

    public static void TrySetProperty<TObject, TValue>(TObject obj, Expression<Func<TObject, TValue>> propertySelector,
        Func<TObject, TValue> valueFactory, params Type[] ignoreAttributeTypes)
    {
        var cacheKey = obj?.GetType().FullName + "-" + $"{propertySelector}-" + (ignoreAttributeTypes != null
            ? "-" + string.Join("-", ignoreAttributeTypes.Select(x => x.FullName))
            : "");
        DictionaryExtensions.GetOrAdd(CachedObjectProperties!, cacheKey, delegate
        {
            if (propertySelector.Body.NodeType != ExpressionType.MemberAccess)
            {
                return null;
            }

            var memberExpression = propertySelector.Body.As<MemberExpression>();
            var propertyInfo = obj?.GetType().GetProperties().FirstOrDefault(x =>
                x.Name == memberExpression.Member.Name && x.GetSetMethod(true) != null);
            if (propertyInfo == null)
            {
                return null;
            }

            return ignoreAttributeTypes != null &&
                   ignoreAttributeTypes.Any(ignoreAttribute => propertyInfo.IsDefined(ignoreAttribute, true))
                ? null
                : propertyInfo;
        })?.SetValue(obj, valueFactory(obj));
    }

    [Obsolete("Obsolete")]
    public static byte[]? ToByteArray(this object obj)
    {
        try
        {
            if (obj.IsNull())
            {
                return null;
            }

            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
        catch (Exception)
        {
            return null;
        }
    }
}