using GClaims.Core.Helpers;

namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para <see cref="T:System.Collections.Generic.IList`1" />.
/// </summary>
public static class ListExtensions
{
    public static void InsertRange<T>(this IList<T> source, int index, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Insert(index++, item);
        }
    }

    public static int FindIndex<T>(this IList<T> source, Predicate<T> selector)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (selector(source[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public static void AddFirst<T>(this IList<T> source, T item)
    {
        source.Insert(0, item);
    }

    public static void AddLast<T>(this IList<T> source, T item)
    {
        source.Insert(source.Count, item);
    }

    public static void InsertAfter<T>(this IList<T> source, T existingItem, T item)
    {
        var index = source.IndexOf(existingItem);
        if (index < 0)
        {
            source.AddFirst(item);
        }
        else
        {
            source.Insert(index + 1, item);
        }
    }

    public static void InsertAfter<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        var index = source.FindIndex(selector);
        if (index < 0)
        {
            source.AddFirst(item);
        }
        else
        {
            source.Insert(index + 1, item);
        }
    }

    public static void InsertBefore<T>(this IList<T> source, T existingItem, T item)
    {
        var index = source.IndexOf(existingItem);
        if (index < 0)
        {
            source.AddLast(item);
        }
        else
        {
            source.Insert(index, item);
        }
    }

    public static void InsertBefore<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        var index = source.FindIndex(selector);
        if (index < 0)
        {
            source.AddLast(item);
        }
        else
        {
            source.Insert(index, item);
        }
    }

    public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (selector(source[i]))
            {
                source[i] = item;
            }
        }
    }

    public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
    {
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (selector(item))
            {
                source[i] = itemFactory(item);
            }
        }
    }

    public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (selector(source[i]))
            {
                source[i] = item;
                break;
            }
        }
    }

    public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
    {
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (selector(item))
            {
                source[i] = itemFactory(item);
                break;
            }
        }
    }

    public static void ReplaceOne<T>(this IList<T> source, T item, T replaceWith)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (Comparer<T>.Default.Compare(source[i], item) == 0)
            {
                source[i] = replaceWith;
                break;
            }
        }
    }

    public static void MoveItem<T>(this List<T> source, Predicate<T> selector, int targetIndex)
    {
        if (!targetIndex.IsBetween(0, source.Count - 1))
        {
            throw new IndexOutOfRangeException("targetIndex should be between 0 and " + (source.Count - 1));
        }

        var currentIndex = source.FindIndex(0, selector);
        if (currentIndex != targetIndex)
        {
            var item = source[currentIndex];
            source.RemoveAt(currentIndex);
            source.Insert(targetIndex, item);
        }
    }

    public static T GetOrAdd<T>(this IList<T> source, Func<T, bool> selector, Func<T> factory)
    {
        Check.NotNull(source, "source");
        var item = source.FirstOrDefault(selector);
        if (item == null)
        {
            item = factory();
            source.Add(item);
        }

        return item;
    }

    /// <summary>
    /// Ordena uma lista por uma ordenação topológica, que considera suas dependências.
    /// </summary>
    /// <typeparam name="T">O tipo dos membros dos valores.</typeparam>
    /// <param name="source">Uma lista de objetos para classificar</param>
    /// <param name="getDependencies">Função para resolver as dependências</param>
    /// <param name="comparer">Comparador de igualdade para dependências </param>
    /// <returns>
    /// Retorna uma nova lista ordenada por dependências.
    /// Se A depende de B, então B virá antes de A na lista resultante.
    /// </returns>
    public static List<T> SortByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies,
        IEqualityComparer<T>? comparer = null) where T : notnull
    {
        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>(comparer);
        foreach (var item in source)
        {
            SortByDependenciesVisit(item, getDependencies, sorted, visited);
        }

        return sorted;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T">O tipo dos membros dos valores.</typeparam>
    /// <param name="item">Item a ser resolvido</param>
    /// <param name="getDependencies">Função para resolver as dependências</param>
    /// <param name="sorted">Lista com os itens de classificação</param>
    /// <param name="visited">Dicionário com os itens visitados</param>
    private static void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted,
        IDictionary<T, bool> visited) where T : notnull
    {
        if (visited.TryGetValue(item, out var inProcess))
        {
            if (!inProcess)
            {
                return;
            }

            throw new ArgumentException("Cyclic dependency found! Item: " + item);
        }

        visited[item] = true;
        var dependencies = getDependencies(item);
        if (dependencies != null)
        {
            foreach (var item2 in dependencies)
            {
                SortByDependenciesVisit(item2, getDependencies, sorted, visited);
            }
        }

        visited[item] = false;
        sorted.Add(item);
    }
}