using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GClaims.Core.Middlewares;

public class RequestLogMiddleware
{
    private readonly ILogger _logger;

    private readonly RequestDelegate _next;

    private Func<LogData, string> _logLineFormatter;

    public RequestLogMiddleware(RequestDelegate next, ILoggerFactory factory)
    {
        _next = next;
        _logger = factory.CreateLogger("RequestLog");
    }

    private Func<LogData, string> logLineFormatter => _logLineFormatter ?? DefaultFormatter();

    /// <summary>
    /// Override this to set the default formatter if none was supplied
    /// </summary>
    /// <returns></returns>
    protected Func<LogData, string> DefaultFormatter()
    {
        return logData =>
            $"\x1B[44m {logData.RequestMethod} - {logData.RequestPath} - {logData.ResponseStatus} - {logData.DurationMs}ms \x1B[40m";
    }

    /// <summary>
    /// Used to set a custom formatter for this instance
    /// </summary>
    /// <param name="formatter"></param>
    public void SetLogLineFormat(Func<LogData, string> formatter)
    {
        _logLineFormatter = formatter;
    }

    public async Task Invoke(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        await _next.Invoke(context);
        watch.Stop();

        var request = context.Request.Path + (string.IsNullOrEmpty(context.Request.QueryString.ToString())
            ? ""
            : context.Request.QueryString.ToString());
        var responseStatus = context.Response.StatusCode;
        var duration = watch.ElapsedMilliseconds;
        var remoteAddr = context.Connection.RemoteIpAddress;
        var method = context.Request.Method;

        var logData = new LogData
        {
            RemoteAddr = remoteAddr,
            RequestMethod = method,
            RequestPath = request,
            ResponseStatus = responseStatus,
            DurationMs = duration
        };

        _logger.LogInformation(logLineFormatter(logData));
    }

    public class LogData
    {
        public IPAddress RemoteAddr { get; set; }

        public string User { get; set; }

        public int ResponseStatus { get; init; }

        public string RequestMethod { get; init; }

        public string RequestTimestamp { get; set; }

        public string RequestPath { get; init; }

        public string RequestProtocol { get; set; }

        public string UserAgent { get; set; }

        public long DurationMs { get; init; }
    }
}