using GClaims.Core.Extensions;

namespace GClaims.Core.Filters.CustomExceptions;

/// <summary>
/// This class will allow to generate the custom exception message.
/// </summary>
public class CustomException : Exception
{
    public CustomException()
    {
        ValidationError = new ValidationError();
    }

    public CustomException(string message) : base(message)
    {
        ValidationError = new ValidationError
        {
            UserMessage = message
        };
    }

    public CustomException(ValidationError validationError) : base(
        validationError.UserMessage ?? validationError.ExceptionMessage, validationError.InnerException)
    {
        ValidationError = validationError;
    }

    public CustomException(string userMessage, string modelName, Exception exception, bool createLog = true) : base(
        exception.Message, exception.InnerException)
    {
        ValidationError = new ValidationError
        {
            Exception = exception,
            ExceptionMessage = exception.Message,
            InnerException = exception.InnerException,
            InnerExceptionMessage = exception.InnerException?.Message,
            ModelName = modelName,
            UserMessage = userMessage,
            CreateLog = createLog
        };

        if (exception.IsNull() && userMessage.IsNullOrEmpty())
        {
            ValidationError.UserMessage = "Falha ao procesar requisição!";
        }
    }

    public CustomException(Exception? exception, bool createLog = true, string? userMessage = null) : base(
        exception?.Message, exception?.InnerException)
    {
        ValidationError = new ValidationError();

        if (exception != null)
        {
            ValidationError.Exception = exception;
            ValidationError.ExceptionMessage = exception.Message;
            ValidationError.InnerException = exception.InnerException;
            ValidationError.InnerExceptionMessage = exception.InnerException?.Message;
        }

        ValidationError.UserMessage = userMessage;
        ValidationError.CreateLog = createLog;
        if (exception.IsNull() && userMessage.IsNullOrEmpty())
        {
            ValidationError.UserMessage = "Falha ao procesar requisição!";
        }
    }

    public CustomException(Exception? exception, string? userMessage = null) : base(exception?.Message,
        exception?.InnerException)
    {
        ValidationError = new ValidationError();

        if (exception != null)
        {
            ValidationError.Exception = exception;
            ValidationError.ExceptionMessage = exception.Message;
            ValidationError.InnerException = exception.InnerException;
            ValidationError.InnerExceptionMessage = exception.InnerException?.Message;
        }

        ValidationError.UserMessage = userMessage;
        ValidationError.CreateLog = true;
        if (exception.IsNull() && userMessage.IsNullOrEmpty())
        {
            ValidationError.UserMessage = "Falha ao procesar requisição!";
        }
    }

    public ValidationError ValidationError { get; set; }
}