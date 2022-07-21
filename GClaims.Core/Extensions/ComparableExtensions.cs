namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para <see cref="T:System.IComparable`1" />.
/// </summary>
public static class ComparableExtensions
{
    /// <summary>
    /// Verifica se um valor está entre um valor mínimo e máximo.
    /// </summary>
    /// <param name="value">O valor a ser verificado</param>
    /// <param name="minInclusiveValue">Valor mínimo (inclusive)</param>
    /// <param name="maxInclusiveValue">Valor máximo (inclusive)</param>
    public static bool IsBetween<T>(this T value, T minInclusiveValue, T maxInclusiveValue) where T : IComparable<T>
    {
        if (value.CompareTo(minInclusiveValue) >= 0)
        {
            return value.CompareTo(maxInclusiveValue) <= 0;
        }

        return false;
    }
}