using GClaims.Core.Helpers;

namespace GClaims.Core.Types;

/// <summary>
/// Esta classe pode ser usada para fornecer uma ação quando
/// O método Dipose é chamado.
/// </summary>
public class DisposeAction : IDisposable
{
    private readonly Action _action;

    /// <summary>
    /// Cria um novo objeto <see cref="T:GClaims.Core.Types.DisposeAction" />.
    /// </summary>
    /// <param name="action">Ação a ser executada quando este objeto for descartado.</param>
    public DisposeAction(Action action)
    {
        Check.NotNull(action, "action");
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}