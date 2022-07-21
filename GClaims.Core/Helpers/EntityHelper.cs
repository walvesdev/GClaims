using System.Linq.Expressions;
using System.Reflection;
using GClaims.Core.Extensions;
using GClaims.Core.Filters.CustomExceptions;

namespace GClaims.Core.Helpers;

public static class EntityHelper
{
    public static bool EntityEquals(Type entity1, Type entity2)
    {
        if (entity1 == null || entity2 == null)
        {
            return false;
        }

        if (ReferenceEquals(entity1, entity2))
        {
            return true;
        }

        if (!entity1.IsAssignableFrom(entity2) && !entity2.IsAssignableFrom(entity1))
        {
            return false;
        }

        if (HasDefaultKeys(entity1) && HasDefaultKeys(entity2))
        {
            return false;
        }

        var entity1Keys = entity1.GetKeys();
        var entity2Keys = entity2.GetKeys();

        if (entity1Keys.Length != entity2Keys.Length)
        {
            return false;
        }

        for (var i = 0; i < entity1Keys.Length; i++)
        {
            var entity1Key = entity1Keys[i];
            var entity2Key = entity2Keys[i];

            if (entity1Key == null)
            {
                if (entity2Key == null)
                {
                    continue;
                }

                return false;
            }

            if (entity2Key == null)
            {
                return false;
            }

            if (TypeHelper.IsDefaultValue(entity1Key) && TypeHelper.IsDefaultValue(entity2Key))
            {
                return false;
            }

            if (!entity1Key.Equals(entity2Key))
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsEntity(Type type)
    {
        Check.NotNull(type, nameof(type));
        return type.Namespace is "GClaims.Domain";
    }

    public static void CheckEntity(Type type)
    {
        Check.NotNull(type, nameof(type));
        if (!IsEntity(type))
        {
            throw new CustomException($"Given {nameof(type)} is not an ef entity: {type.AssemblyQualifiedName}.");
        }
    }

    public static bool IsEntityWithId(Type type)
    {
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.GetTypeInfo().IsGenericType)
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasDefaultId<TKey>(Type entity)
    {
        var entityId = entity.GetKeys()[0];
        var idValue = Convert.ChangeType(entityId, typeof(TKey));

        if (idValue.IsNotNull() && EqualityComparer<TKey>.Default.Equals((TKey)idValue, default))
        {
            return true;
        }

        if (idValue.IsNotNull() && typeof(TKey) == typeof(int))
        {
            return Convert.ToInt32(idValue) <= 0;
        }

        if (idValue.IsNotNull() && typeof(TKey) == typeof(long))
        {
            return Convert.ToInt64(idValue) <= 0;
        }

        return false;
    }

    private static bool IsDefaultKeyValue(object value)
    {
        if (value == null)
        {
            return true;
        }

        var type = value.GetType();

        if (type == typeof(int))
        {
            return Convert.ToInt32(value) <= 0;
        }

        if (type == typeof(long))
        {
            return Convert.ToInt64(value) <= 0;
        }

        return TypeHelper.IsDefaultValue(value);
    }

    public static bool HasDefaultKeys(Type entity)
    {
        Check.NotNull(entity, nameof(entity));

        return entity.GetKeys().All(key => key == null || IsDefaultKeyValue(key));
    }

    /// <summary>
    /// Tries to find the primary key type of the given entity type.
    /// May return null if given type does not implement <see cref="Type{TKey}" />
    /// </summary>
    public static Type? FindPrimaryKeyType<TEntity>()
        where TEntity : class, new()
    {
        return FindPrimaryKeyType(typeof(TEntity));
    }

    /// <summary>
    /// Tries to find the primary key type of the given entity type.
    /// May return null if given type does not implement <see cref="Type{TKey}" />
    /// </summary>
    public static Type? FindPrimaryKeyType(Type entityType)
    {
        return (from interfaceType in entityType.GetTypeInfo().GetInterfaces()
            where interfaceType.GetTypeInfo().IsGenericType
            select interfaceType.GenericTypeArguments[0]).FirstOrDefault();
    }

    public static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity, TKey>(TKey id)
        where TEntity : class, new()
    {
        var type = typeof(TEntity);
        var lambdaParam = Expression.Parameter(type);
        var leftExpression = Expression.PropertyOrField(lambdaParam, $"{type.GetTypeName()}Id");
        var idValue = Convert.ChangeType(id, typeof(TKey));
        Expression<Func<object>> closure = () => idValue!;
        var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);
        var lambdaBody = Expression.Equal(leftExpression, rightExpression);
        return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
    }
}