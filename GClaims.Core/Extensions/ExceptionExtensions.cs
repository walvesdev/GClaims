using System.Runtime.ExceptionServices;

namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para a classe <see cref="T:System.Exception" />.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Usa o método <see cref="M:System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(System.Exception)" /> para
    /// relançar a exceção
    /// preservando o rastreamento de pilha.
    /// </summary>
    /// <param name="exception">Exceção a ser relançada</param>
    public static void ReThrow(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}