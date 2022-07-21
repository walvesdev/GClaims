using System.Globalization;
using GClaims.Core.Extensions;
using GClaims.Core.Types;

namespace GClaims.Core.Helpers;

public static class CultureHelper
{
    public static bool IsRtl => CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    public static IDisposable Use(string culture, string? uiCulture = null)
    {
        Check.NotNull(culture, "culture");
        return Use(new CultureInfo(culture), uiCulture == null ? null : new CultureInfo(uiCulture));
    }

    public static IDisposable Use(CultureInfo culture, CultureInfo? uiCulture = null)
    {
        Check.NotNull(culture, "culture");
        var currentCulture = CultureInfo.CurrentCulture;
        var currentUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = uiCulture ?? culture;
        return new DisposeAction(delegate
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUiCulture;
        });
    }

    public static bool IsValidCultureCode(string cultureCode)
    {
        if (cultureCode.IsNullOrWhiteSpace())
        {
            return false;
        }

        try
        {
            _ = CultureInfo.GetCultureInfo(cultureCode);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    public static string GetBaseCultureName(string cultureName)
    {
        return !cultureName.Contains('-')
            ? cultureName
            : cultureName.Left(cultureName.IndexOf("-", StringComparison.Ordinal));
    }
}