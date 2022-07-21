using System.Reflection;

namespace GClaims.Core.Helpers;

public static class InternalAsyncHelper
{
    public static async Task AwaitTaskWithFinally(Task actualReturnValue, Action<Exception> finalAction)
    {
        Exception exception = null;
        try
        {
            await actualReturnValue.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            if (exception != null)
            {
                finalAction(exception);
            }
        }
    }

    public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction,
        Action<Exception> finalAction)
    {
        Exception exception = null;
        try
        {
            await actualReturnValue.ConfigureAwait(false);
            await postAction().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            if (exception != null)
            {
                finalAction(exception);
            }
        }
    }

    public static async Task AwaitTaskWithPreActionAndPostActionAndFinally(Func<Task> actualReturnValue,
        Func<Task>? preAction = null, Func<Task>? postAction = null, Action<Exception>? finalAction = null)
    {
        Exception exception = null;
        try
        {
            if (preAction != null)
            {
                await preAction().ConfigureAwait(false);
            }

            await actualReturnValue().ConfigureAwait(false);
            if (postAction != null)
            {
                await postAction().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            if (exception != null)
            {
                finalAction?.Invoke(exception);
            }
        }
    }

    public static async Task<T> AwaitTaskWithFinallyAndGetResult<T>(Task<T> actualReturnValue,
        Action<Exception> finalAction)
    {
        Exception exception = null;
        try
        {
            return await actualReturnValue.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            if (exception != null)
            {
                finalAction(exception);
            }
        }
    }

    public static object? CallAwaitTaskWithFinallyAndGetResult(Type taskReturnType, object actualReturnValue,
        Action<Exception> finalAction)
    {
        return typeof(InternalAsyncHelper).GetTypeInfo().GetMethod("AwaitTaskWithFinallyAndGetResult",
                BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(taskReturnType)
            .Invoke(null, new[] { actualReturnValue, finalAction });
    }

    public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue,
        Func<Task> postAction, Action<Exception> finalAction)
    {
        Exception exception = null;
        try
        {
            var result = await actualReturnValue.ConfigureAwait(false);
            await postAction().ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            if (exception != null)
            {
                finalAction(exception);
            }
        }
    }

    public static object? CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType,
        object actualReturnValue, Func<Task> action, Action<Exception> finalAction)
    {
        return typeof(InternalAsyncHelper).GetTypeInfo().GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult",
                BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(taskReturnType)
            .Invoke(null, new[] { actualReturnValue, action, finalAction });
    }

    public static async Task<T> AwaitTaskWithPreActionAndPostActionAndFinallyAndGetResult<T>(
        Func<Task<T>> actualReturnValue, Func<Task>? preAction = null, Func<Task>? postAction = null,
        Action<Exception>? finalAction = null)
    {
        Exception exception = null;
        try
        {
            if (preAction != null)
            {
                await preAction().ConfigureAwait(false);
            }

            var result = await actualReturnValue().ConfigureAwait(false);
            if (postAction != null)
            {
                await postAction().ConfigureAwait(false);
            }

            return result;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            if (exception != null)
            {
                finalAction?.Invoke(exception);
            }
        }
    }

    public static object? CallAwaitTaskWithPreActionAndPostActionAndFinallyAndGetResult(Type taskReturnType,
        Func<object> actualReturnValue, Func<Task>? preAction = null, Func<Task>? postAction = null,
        Action<Exception>? finalAction = null)
    {
        var returnFunc =
            typeof(InternalAsyncHelper).GetTypeInfo().GetMethod("ConvertFuncOfObjectToFuncOfTask",
                    BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue });
        return typeof(InternalAsyncHelper).GetTypeInfo().GetMethod(
                "AwaitTaskWithPreActionAndPostActionAndFinallyAndGetResult", BindingFlags.Static | BindingFlags.Public)!
            .MakeGenericMethod(taskReturnType)
            .Invoke(null, new[] { returnFunc, preAction, postAction, finalAction });
    }

    public static Func<Task<T>> ConvertFuncOfObjectToFuncOfTask<T>(Func<object> actualReturnValue)
    {
        return () => (Task<T>)actualReturnValue();
    }
}