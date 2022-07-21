namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para <see cref="T:System.EventHandler" />.
/// </summary>
public static class EventHandlerExtensions
{
    /// <summary>
    /// Gera determinado evento com segurança com os argumentos fornecidos.
    /// </summary>
    /// <param name="eventHandler">O manipulador de eventos</param>
    /// <param name="sender">Fonte do evento</param>
    public static void InvokeSafely(this EventHandler eventHandler, object sender)
    {
        eventHandler.InvokeSafely(sender, EventArgs.Empty);
    }

    /// <summary>
    /// Gera determinado evento com segurança com os argumentos fornecidos.
    /// </summary>
    /// <param name="eventHandler">O manipulador de eventos</param>
    /// <param name="sender">Fonte do evento</param>
    /// <param name="e">Argumento do evento</param>
    public static void InvokeSafely(this EventHandler eventHandler, object sender, EventArgs e)
    {
        eventHandler?.Invoke(sender, e);
    }

    /// <summary>
    /// Gera determinado evento com segurança com os argumentos fornecidos.
    /// </summary>
    /// <typeparam name="TEventArgs">Tipo do <see cref="T:System.EventArgs" /></typeparam>
    /// <param name="eventHandler">O manipulador de eventos</param>
    /// <param name="sender">Fonte do evento</param>
    /// <param name="e">Argumento do evento</param>
    public static void InvokeSafely<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
        where TEventArgs : EventArgs
    {
        eventHandler?.Invoke(sender, e);
    }
}