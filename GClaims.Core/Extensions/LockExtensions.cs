namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para facilitar o bloqueio.
/// </summary>
public static class LockExtensions
{
    /// <summary>
    /// Executa dado <paramref name="action" /> bloqueando dado <paramref name="source" /> objeto.
    /// </summary>
    /// <param name="source">Objeto de origem (a ser bloqueado)</param>
    /// <param name="action">Ação (a ser executada)</param>
    public static void Locking(this object source, Action action)
    {
        lock (source)
        {
            action();
        }
    }

    /// <summary>
    /// Executa dado <paramref name="action" /> bloqueando dado <paramref name="source" /> objeto.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto (a ser bloqueado)</typeparam>
    /// <param name="source">Objeto de origem (a ser bloqueado)</param>
    /// <param name="action">Ação (a ser executada)</param>
    public static void Locking<T>(this T source, Action<T> action) where T : class
    {
        lock (source)
        {
            action(source);
        }
    }

    /// <summary>
    /// Executa determinado <paramref name="func" /> e retorna seu valor bloqueando determinado objeto
    /// <paramref name="source" />.
    /// </summary>
    /// <typeparam name="TResult">Tipo de retorno</typeparam>
    /// <param name="source">Objeto de origem (a ser bloqueado)</param>
    /// <param name="func">Função (a ser executada)</param>
    /// <returns>Valor de retorno do <paramref name="func" /></returns>
    public static TResult Locking<TResult>(this object source, Func<TResult> func)
    {
        lock (source)
        {
            return func();
        }
    }

    /// <summary>
    /// Executa determinado <paramref name="func" /> e retorna seu valor bloqueando determinado objeto
    /// <paramref name="source" />.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto (a ser bloqueado)</typeparam>
    /// <typeparam name="TResult">Tipo de retorno</typeparam>
    /// <param name="source">Objeto de origem (a ser bloqueado)</param>
    /// <param name="func">Função (a ser executada)</param>
    /// <returns>Valor de retorno do <paramnref name="func" /></returns>
    public static TResult Locking<T, TResult>(this T source, Func<T, TResult> func) where T : class
    {
        lock (source)
        {
            return func(source);
        }
    }
}