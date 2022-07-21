using Nager.Date;
using Nager.Date.Extensions;

namespace GClaims.Core.Extensions;

public static class DateExtensions
{
    private const CountryCode COUNTRY_CODE = CountryCode.BR;

    public static DateTime GetDateOrNextBusinessDay(this DateTime date)
    {
        if (date == DateTime.MinValue)
        {
            date = DateTime.Today;
        }

        while (!date.IsBusinessDay())
        {
            date = date.AddDays(1);
        }

        return date;
    }

    public static bool IsBusinessDay(this DateTime date)
    {
        if (date == DateTime.MinValue)
        {
            return false;
        }

        return !DateSystem.IsPublicHoliday(date, COUNTRY_CODE) && !date.IsWeekend(COUNTRY_CODE);
    }
}