namespace GClaims.Core.Helpers;

internal static class NumberHelper
{
    public static decimal IntegerToDecimal(object value)
    {
        return Convert.ToDecimal(value) / 100;
    }

    public static double IntegerToDouble(object value)
    {
        return Convert.ToDouble(value) / 100;
    }

    public static object DecimalToInteger(object value)
    {
        return Convert.ToInt32(Convert.ToDecimal(value) * 100);
    }

    public static object DoubleToInteger(object value)
    {
        return Convert.ToInt32(Convert.ToDouble(value) * 100);
    }
}