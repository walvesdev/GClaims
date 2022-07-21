namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para o<see cref="T:System.DayOfWeek" />.
/// </summary>
public static class DayOfWeekExtensions
{
    /// <summary>
    /// Verifique se o valor fornecido <see cref="T:System.DayOfWeek" /> é final de semana.
    /// </summary>
    public static bool IsWeekend(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek.IsIn(DayOfWeek.Saturday, DayOfWeek.Sunday);
    }

    /// <summary>
    /// Verifique se o valor fornecido <see cref="T:System.DayOfWeek" /> é o dia da semana.
    /// </summary>
    public static bool IsWeekday(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek.IsIn(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
            DayOfWeek.Friday);
    }
}