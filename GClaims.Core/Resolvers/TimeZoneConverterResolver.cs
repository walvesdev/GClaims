using Hangfire;
using TimeZoneConverter;

namespace GClaims.Core.Resolvers
{
    public sealed class TimeZoneConverterResolver : ITimeZoneResolver
    {
        public TimeZoneInfo GetTimeZoneById(string timeZoneId)
        {
            return TZConvert.GetTimeZoneInfo(timeZoneId);
        }
    }
}
