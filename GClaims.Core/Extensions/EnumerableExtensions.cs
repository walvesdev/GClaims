namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para <see cref="T:System.Collections.Generic.IEnumerable`1" />.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Concatena os membros de uma coleção <see cref="T:System.Collections.Generic.IEnumerable`1" /> construída do tipo
    /// System.String, usando o separador especificado entre cada membro.
    /// Este é um atalho para string.Join(...)
    /// </summary>
    /// <param name="source">Uma coleção que contém as strings a serem concatenadas.</param>
    /// <param name="separator">
    /// A string a ser usada como separador. separador é incluído na string retornada somente se os
    /// valores tiverem mais de um elemento.
    /// </param>
    /// <returns>
    /// Uma string que consiste nos membros de valores delimitados pela string separadora. Se valores não tiverem
    /// membros, o método retornará System.String.Empty.
    /// </returns>
    public static string JoinAsString(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// Concatena os membros de uma coleção, usando o separador especificado entre cada membro.
    /// Este é um atalho para string.Join(...)
    /// </summary>
    /// <param name="source">Uma coleção que contém os objetos a serem concatenados.</param>
    /// <param name="separator">
    /// A string a ser usada como separador. separador é incluído na string retornada somente se os
    /// valores tiverem mais de um elemento.
    /// </param>
    /// <typeparam name="T">O tipo dos membros dos valores.</typeparam>
    /// <returns>
    /// Uma string que consiste nos membros de valores delimitados pela string separadora. Se valores não tiverem
    /// membros, o método retornará System.String.Empty.
    /// </returns>
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// Filtra um <see cref="T:System.Collections.Generic.IEnumerable`1" /> pelo predicado dado se a condição dada for
    /// verdadeira.
    /// </summary>
    /// <param name="source">Enumerável para aplicar filtragem</param>
    /// <param name="condition">Um valor booleano</param>
    /// <param name="predicate">Predicado para filtrar o enumerável</param>
    /// <returns>Filtrado ou não filtrado enumerável baseado em <paramref name="condition" /></returns>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        if (!condition)
        {
            return source;
        }

        return source.Where(predicate);
    }

    /// <summary>
    /// Filtra um <see cref="T:System.Collections.Generic.IEnumerable`1" /> pelo predicado dado se a condição dada for
    /// verdadeira.
    /// </summary>
    /// <param name="source">Enumerável para aplicar filtragem</param>
    /// <param name="condition">Um valor booleano</param>
    /// <param name="predicate">Predicado para filtrar o enumerável</param>
    /// <returns>Filtrado ou não filtrado enumerável baseado em <paramref name="condition" /></returns>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
    {
        if (!condition)
        {
            return source;
        }

        return source.Where(predicate);
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> itemAction)
    {
        foreach (var item in items)
        {
            itemAction(item);
        }
    }
}