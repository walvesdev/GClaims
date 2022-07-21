namespace GClaims.Core.Helpers;

/// <summary>
/// Um atalho para usar a classe <see cref="T:System.Random" />.
/// Também fornece alguns métodos úteis.
/// </summary>
public static class RandomHelper
{
    private static readonly Random Rnd = new Random();

    /// <summary>
    /// Retorna um número aleatório dentro de um intervalo especificado.
    /// </summary>
    /// <param name="minValue">O limite inferior inclusivo do número aleatório retornado.</param>
    /// <param name="maxValue">
    /// O limite superior exclusivo do número aleatório retornado. maxValue deve ser maior ou igual a
    /// minValue.
    /// </param>
    /// <returns>
    /// Um inteiro com sinal de 32 bits maior ou igual a minValue e menor que maxValue;
    /// ou seja, o intervalo de valores de retorno inclui minValue, mas não maxValue.
    /// Se minValue for igual a maxValue, minValue será retornado.
    /// </returns>
    public static int GetRandom(int minValue, int maxValue)
    {
        lock (Rnd)
        {
            return Rnd.Next(minValue, maxValue);
        }
    }

    /// <summary>
    /// Retorna um número aleatório não negativo menor que o máximo especificado.
    /// </summary>
    /// <param name="maxValue">
    /// O limite superior exclusivo do número aleatório a ser gerado. maxValue deve ser maior ou igual a
    /// zero.
    /// </param>
    /// <returns>
    /// Um inteiro com sinal de 32 bits maior ou igual a zero e menor que maxValue;
    /// ou seja, o intervalo de valores de retorno normalmente inclui zero, mas não maxValue.
    /// No entanto, se maxValue for igual a zero, maxValue será retornado.
    /// </returns>
    public static int GetRandom(int maxValue)
    {
        lock (Rnd)
        {
            return Rnd.Next(maxValue);
        }
    }

    /// <summary>
    /// Retorna um número aleatório não negativo.
    /// </summary>
    /// <returns>Um inteiro com sinal de 32 bits maior ou igual a zero e menor que <see cref="F:System.Int32.MaxValue" />.</returns>
    public static int GetRandom()
    {
        lock (Rnd)
        {
            return Rnd.Next();
        }
    }

    /// <summary>
    /// Obtém aleatoriamente os objetos fornecidos.
    /// </summary>
    /// <typeparam name="T">Tipo dos objetos</typeparam>
    /// <param name="objs">Lista de objetos para selecionar um aleatório</param>
    public static T GetRandomOf<T>(params T[] objs)
    {
        Check.NotNullOrEmpty(objs, "objs");
        return objs[GetRandom(0, objs.Length)];
    }

    /// <summary>
    /// Obtém um item aleatório da lista fornecida.
    /// </summary>
    /// <typeparam name="T">Tipo dos objetos</typeparam>
    /// <param name="list">Lista de objetos para selecionar um aleatório</param>
    public static T GetRandomOfList<T>(IList<T> list)
    {
        Check.NotNullOrEmpty(list, "list");
        return list[GetRandom(0, list.Count)];
    }

    /// <summary>
    /// Gera uma lista aleatória de um dado enumerável.
    /// </summary>
    /// <typeparam name="T">Tipo de itens na lista</typeparam>
    /// <param name="items">itens</param>
    public static List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
    {
        Check.NotNull(items, "items");
        var currentList = new List<T>(items);
        var randomList = new List<T>();
        while (currentList.Any())
        {
            var randomIndex = GetRandom(0, currentList.Count);
            randomList.Add(currentList[randomIndex]);
            currentList.RemoveAt(randomIndex);
        }

        return randomList;
    }
}