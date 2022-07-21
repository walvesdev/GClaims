using System.Reflection;
using Nito.AsyncEx;

namespace GClaims.Core.Helpers;

/// <summary>
/// Fornece alguns métodos auxiliares para trabalhar com métodos assíncronos.
/// </summary>
public static class AsyncHelper
{
    /// <summary>
    /// Verifica se o método fornecido é um método assíncrono.
    /// </summary>
    /// <param name="method">Um método para verificar</param>
    public static bool IsAsync(this MethodInfo method)
    {
        Check.NotNull(method, "method");
        return IsTaskOrTaskOfT(method.ReturnType);
    }

    public static bool IsTaskOrTaskOfT(this Type type)
    {
        if (!(type == typeof(Task)))
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(Task<>);
            }

            return false;
        }

        return true;
    }

    public static bool IsTaskOfT(this Type type)
    {
        if (type.GetTypeInfo().IsGenericType)
        {
            return type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        return false;
    }

    /// <summary>
    /// Retorna void se o tipo fornecido for Task.
    /// Retorna T, se o tipo fornecido for Task{T}.
    /// Retorna o tipo dado caso contrário.
    /// </summary>
    public static Type UnwrapTask(Type type)
    {
        Check.NotNull(type, "type");
        if (type == typeof(Task))
        {
            return typeof(void);
        }

        if (IsTaskOfT(type))
        {
            return type.GenericTypeArguments[0];
        }

        return type;
    }

    /// <summary>
    /// Executa um método assíncrono de forma síncrona.
    /// </summary>
    /// <param name="func">Uma função que retorna um resultado</param>
    /// <typeparam name="TResult">Tipo de resultado</typeparam>
    /// <returns>Resultado da operação assíncrona</returns>
    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        return AsyncContext.Run(func);
    }

    /// <summary>
    /// Executa um método assíncrono de forma síncrona.
    /// </summary>
    /// <param name="action">Uma ação assíncrona</param>
    public static void RunSync(Func<Task> action)
    {
        AsyncContext.Run(action);
    }
}