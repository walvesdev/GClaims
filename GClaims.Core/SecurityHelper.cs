using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GClaims.Core;

public static class SecurityHelper
{
    public static string GenerateRandomPassword(PasswordOptions? opts = null)
    {
        if (opts == null)
        {
            opts = new PasswordOptions
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };
        }

        string[] randomChars =
        {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase 
            "abcdefghijkmnopqrstuvwxyz", // lowercase
            "0123456789", // digits
            "!@$?_-" // non-alphanumeric
        };
        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();

        if (opts.RequireUppercase)
        {
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[0][rand.Next(0, randomChars[0].Length)]);
        }

        if (opts.RequireLowercase)
        {
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[1][rand.Next(0, randomChars[1].Length)]);
        }

        if (opts.RequireDigit)
        {
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[2][rand.Next(0, randomChars[2].Length)]);
        }

        if (opts.RequireNonAlphanumeric)
        {
            chars.Insert(rand.Next(0, chars.Count),
                randomChars[3][rand.Next(0, randomChars[3].Length)]);
        }

        for (var i = chars.Count;
             i < opts.RequiredLength
             || chars.Distinct().Count() < opts.RequiredUniqueChars;
             i++)
        {
            var rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count),
                rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }
}

public static class ContextExtensions
{
    public static bool IsDisposed(this DbContext context)
    {
        var result = true;

        var typeDbContext = typeof(DbContext);
        var typeInternalContext = typeDbContext.Assembly.GetType("System.Data.Entity.Internal.InternalContext");

        var fi_InternalContext =
            typeDbContext.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance);
        var pi_IsDisposed = typeInternalContext?.GetProperty("IsDisposed");

        var ic = fi_InternalContext?.GetValue(context);

        if (ic != null)
        {
            result = (bool)pi_IsDisposed?.GetValue(ic)!;
        }

        return result;
    }
}