using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para Dicionário.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Este método é usado para tentar obter um valor em um dicionário se ele existir.
    /// </summary>
    /// <typeparam name="T">Tipo do valor</typeparam>
    /// <param name="dictionary">O objeto de coleção</param>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor da chave (ou valor padrão se a chave não existir)</param>
    /// <returns>Verdadeiro se a chave existe no dicionário</returns>
    internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T? value)
    {
        if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T obj)
        {
            value = obj;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Obtém um valor do dicionário com a chave fornecida. Retorna o valor padrão se não encontrar.
    /// </summary>
    /// <param name="dictionary">Dicionário para verificar e obter</param>
    /// <param name="key">Chave para encontrar o valor</param>
    /// <typeparam name="TKey">Tipo da chave</typeparam>
    /// <typeparam name="TValue">Tipo do valor</typeparam>
    /// <returns>Valor se encontrado, padrão se não encontrado.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        return !dictionary.TryGetValue(key, out var obj) ? default! : obj;
    }

    /// <summary>
    /// Obtém um valor do dicionário com a chave fornecida. Retorna o valor padrão se não encontrar.
    /// </summary>
    /// <param name="dictionary">Dicionário para verificar e obter</param>
    /// <param name="key">Chave para encontrar o valor</param>
    /// <typeparam name="TKey">Tipo da chave</typeparam>
    /// <typeparam name="TValue">Tipo do valor</typeparam>
    /// <returns>Valor se encontrado, padrão se não encontrado.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return !dictionary.TryGetValue(key, out var obj) ? default! : obj;
    }

    /// <summary>
    /// Obtém um valor do dicionário com a chave fornecida. Retorna o valor padrão se não encontrar.
    /// </summary>
    /// <param name="dictionary">Dicionário para verificar e obter</param>
    /// <param name="key">Chave para encontrar o valor</param>
    /// <typeparam name="TKey">Tipo da chave</typeparam>
    /// <typeparam name="TValue">Tipo do valor</typeparam>
    /// <returns>Valor se encontrado, padrão se não encontrado.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        return !dictionary.TryGetValue(key, out var obj) ? default! : obj;
    }

    /// <summary>
    /// Obtém um valor do dicionário com a chave fornecida. Retorna o valor padrão se não encontrar.
    /// </summary>
    /// <param name="dictionary">Dicionário para verificar e obter</param>
    /// <param name="key">Chave para encontrar o valor</param>
    /// <typeparam name="TKey">Tipo da chave</typeparam>
    /// <typeparam name="TValue">Tipo do valor</typeparam>
    /// <returns>Valor se encontrado, padrão se não encontrado.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        return !dictionary.TryGetValue(key, out var obj) ? default! : obj;
    }

    /// <summary>
    /// Obtém um valor do dicionário com a chave fornecida. Retorna o valor padrão se não encontrar.
    /// </summary>
    /// <param name="dictionary">Dicionário para verificar e obter</param>
    /// <param name="key">Chave para encontrar o valor</param>
    /// <param name="factory">Um método de fábrica usado para criar o valor se não for encontrado no dicionário</param>
    /// <typeparam name="TKey">Tipo da chave</typeparam>
    /// <typeparam name="TValue">Tipo do valor</typeparam>
    /// <returns>Valor se encontrado, padrão se não encontrado.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TKey, TValue> factory)
    {
        if (dictionary.TryGetValue(key, out var obj))
        {
            return obj;
        }

        return dictionary[key] = factory(key);
    }

    /// <summary>
    /// Obtém um valor do dicionário com a chave fornecida. Retorna o valor padrão se não encontrar.
    /// </summary>
    /// <param name="dictionary">Dicionário para verificar e obter</param>
    /// <param name="key">Chave para encontrar o valor</param>
    /// <param name="factory">Um método de fábrica usado para criar o valor se não for encontrado no dicionário</param>
    /// <typeparam name="TKey">Tipo da chave</typeparam>
    /// <typeparam name="TValue">Tipo do valor</typeparam>
    /// <returns>Valor se encontrado, padrão se não encontrado.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, [DisallowNull] TKey key, Func<TValue> factory) where TKey : notnull
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        return dictionary.GetOrAdd(key, _ => factory());
    }
}