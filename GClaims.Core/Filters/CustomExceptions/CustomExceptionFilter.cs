using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GClaims.Core.Extensions;

namespace GClaims.Core.Filters.CustomExceptions;

/// <summary>
/// Essa classe é usada para manipular a exceção personalizada no nível do aplicativo.
/// </summary>
public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilter> Logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
    {
        Logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception ?? null;
        var hasException = exception != null;
        var hasInnerException = exception?.InnerException != null;

        string errorMessage = null;
        string stackTrace = null;
        string fullStackTrace = null;
        string innerStackTrace = null;
        Exception innerException = null;
        string innerExceptionMessage = null;
        string model = null;
        string userMessage = null;
        object data = null;
        var createLog = true;

        if (hasException)
        {
            if (!exception.Message.Contains("inner exception"))
            {
                errorMessage = exception.Message;
            }

            if (hasInnerException)
            {
                innerStackTrace = new StackTrace(exception.InnerException, true).ToString();
            }

            if (exception.InnerException != null)
            {
                innerException = exception.InnerException;

                if (!exception.InnerException.Message.IsNullOrWhiteSpace())
                {
                    innerExceptionMessage = exception.InnerException.Message;
                }
            }

            fullStackTrace = exception.StackTrace;
            stackTrace = new StackTrace(exception, true).ToString();
        }

        switch (exception)
        {
            case CustomException customException:

                var hasCustomException = customException.ValidationError.Exception != null;
                var hasCustomInnerException = customException.ValidationError.InnerException != null;
                var hasResponseModel = customException.ValidationError != null;

                if (hasResponseModel && hasCustomException)
                {
                    fullStackTrace = customException.ValidationError.Exception.StackTrace;
                    stackTrace = new StackTrace(customException.ValidationError.Exception, true).ToString();
                }

                if (hasResponseModel && hasCustomInnerException)
                {
                    innerStackTrace = new StackTrace(customException.ValidationError.InnerException, true).ToString();
                }

                if (hasResponseModel)
                {
                    var responseModel = customException.ValidationError;

                    if (!responseModel.ModelName.IsNullOrWhiteSpace())
                    {
                        model = responseModel.ModelName;
                    }

                    if (!responseModel.ExceptionMessage.IsNullOrWhiteSpace() &&
                        !responseModel.ExceptionMessage.Contains("inner exception"))
                    {
                        errorMessage = responseModel.ExceptionMessage;
                    }

                    if (!responseModel.UserMessage.IsNullOrWhiteSpace())
                    {
                        userMessage = responseModel.UserMessage;
                    }

                    if (responseModel.Data != null)
                    {
                        data = responseModel.Data;
                    }

                    createLog = customException.ValidationError.CreateLog;
                }

                if (customException.InnerException != null)
                {
                    innerException = customException.InnerException;

                    if (!customException.InnerException.Message.IsNullOrWhiteSpace())
                    {
                        innerExceptionMessage = customException.InnerException.Message;
                    }
                }

                break;
        }

        var response = context.HttpContext.Response;
        response.ContentType = "application/json";

        #region Logging

        if (createLog)
        {
            Logger.LogError(new
            {
                userMessage,
                exceptionmessage = errorMessage,
                innerMessage = innerExceptionMessage,
                responseModel = model,
                source = exception.Source,
                stackTrace,
                fullStackTrace,
                innerStackTrace,
                innerException,
                data
            }.ToString());
        }

        #endregion Logging

        object result = null;

        try
        {
            result = new
            {
                message = !userMessage.IsNullOrWhiteSpace() ? JsonConvert.DeserializeObject(userMessage) : null,
                sistemMessage = userMessage.IsNullOrWhiteSpace() && !errorMessage.IsNullOrWhiteSpace()
                    ? JsonConvert.DeserializeObject(errorMessage)
                    : null,
                innerMessage = userMessage.IsNullOrWhiteSpace() && !innerExceptionMessage.IsNullOrWhiteSpace()
                    ? innerExceptionMessage
                    : null,
                data
            };
        }
        catch (JsonReaderException)
        {
            try
            {
                result = new
                {
                    message = userMessage,
                    sistemMessage = userMessage.IsNullOrWhiteSpace() && !errorMessage.IsNullOrWhiteSpace()
                        ? JsonConvert.DeserializeObject(errorMessage)
                        : null,
                    innerMessage = userMessage.IsNullOrWhiteSpace() && !innerExceptionMessage.IsNullOrWhiteSpace()
                        ? innerExceptionMessage
                        : null,
                    data
                };
            }
            catch (JsonReaderException)
            {
                try
                {
                    result = new
                    {
                        message = !userMessage.IsNullOrWhiteSpace() ? JsonConvert.DeserializeObject(userMessage) : null,
                        sistemMessage = userMessage.IsNullOrWhiteSpace() && !errorMessage.IsNullOrWhiteSpace()
                            ? JsonConvert.DeserializeObject(errorMessage)
                            : null,
                        innerMessage = userMessage.IsNullOrWhiteSpace() && !innerExceptionMessage.IsNullOrWhiteSpace()
                            ? innerExceptionMessage
                            : null,
                        data
                    };
                }
                catch (JsonReaderException)
                {
                    result = new
                    {
                        message = userMessage,
                        sistemMessage = userMessage.IsNullOrWhiteSpace() && !errorMessage.IsNullOrWhiteSpace()
                            ? errorMessage
                            : null,
                        innerMessage = userMessage.IsNullOrWhiteSpace() && !innerExceptionMessage.IsNullOrWhiteSpace()
                            ? innerExceptionMessage
                            : null,
                        data
                    };
                }
            }
        }

        context.Result = new JsonResult(result, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}