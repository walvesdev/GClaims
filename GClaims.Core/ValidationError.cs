using System.Net;

namespace GClaims.Core;

[Serializable]
public class ValidationError
{
    public ValidationError()
    {
    }

    public ValidationError(string message)
    {
        Message = message;
    }

    public string Message { get; set; }

    public string? UserMessage { get; set; }

    public string ExceptionMessage { get; set; }

    public HttpStatusCode? StatusCode { get; set; }

    public string ModelName { get; set; }

    public Exception? Exception { get; set; }

    public Exception? InnerException { get; set; }

    public string? InnerExceptionMessage { get; set; }

    public object Data { get; set; }

    public bool CreateLog { get; internal set; }
}