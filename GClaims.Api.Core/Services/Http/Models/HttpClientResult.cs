using System.Net;
using GClaims.Core.Extensions;

namespace GClaims.Core.Services.Http.Models;

public class HttpClientResult<T>
{
    private string? _errorMessage;
    private Exception _exception;

    public bool IsSuccessStatus { get; set; }

    public T Data { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public HttpResponseMessage HttpResponseMessage { get; set; }

    public string? ErrorMessage
    {
        get
        {
            if (_errorMessage.IsNullOrEmpty() && Exception.IsNull())
            {
                return Enum.GetName(typeof(HttpStatusCode), StatusCode);
            }

            return _errorMessage;
        }
        set
        {
            _errorMessage = value;
            if (!string.IsNullOrWhiteSpace(_errorMessage) && Exception == null)
            {
                AppServices.SetLogErrors<HttpClientResult<T>>(message: _errorMessage.RemoveScape());
            }
        }
    }

    public Exception Exception
    {
        get => _exception;
        set
        {
            _exception = value;
            if (_errorMessage != null)
            {
                AppServices.SetLogErrors<HttpClientResult<T>>(_exception, StatusCode,
                    _errorMessage.RemoveScape());
            }
        }
    }
}