namespace GClaims.Core.Types;

/// <summary>
/// Estende <see cref="T:System.Collections.Generic.IList`1" /> para adicionar restrição a um tipo base específico.
/// </summary>
/// <typeparam name="TBaseType">Tipo base de <see cref="T:System.Type" />s nesta lista</typeparam>
public interface ITypeList<in TBaseType> : IList<Type>
{
    /// <summary>
    /// Adiciona um tipo à lista.
    /// </summary>
    /// <typeparam name="T">Tipo</typeparam>
    void Add<T>() where T : TBaseType;

    /// <summary>
    /// Adiciona um tipo à lista se ainda não estiver na lista.
    /// </summary>
    /// <typeparam name="T">Tipo</typeparam>
    bool TryAdd<T>() where T : TBaseType;

    /// <summary>
    /// Verifica se existe um tipo na lista.
    /// </summary>
    /// <typeparam name="T">Tipo</typeparam>
    /// <returns></returns>
    bool Contains<T>() where T : TBaseType;

    /// <summary>
    /// Remove um tipo da lista
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void Remove<T>() where T : TBaseType;
}