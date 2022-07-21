namespace GClaims.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ClearTime(this DateTime dateTime)
    {
        return dateTime.Subtract(new TimeSpan(0, dateTime.Hour, dateTime.Minute, dateTime.Second,
            dateTime.Millisecond));
    }

    public static DateTime ToFormatedDate(this DateTime dateValue)
    {
        try
        {
            const string pattern = "yyyy-MM-dd-HH-mm-ss";
            return DateTime.ParseExact(dateValue.ToString("yyyy-MM-dd-HH-mm-ss"), pattern, null);
        }
        catch (Exception e)
        {
            throw new Exception("Formato de data inválido! " + e.Message);
        }
    }
}