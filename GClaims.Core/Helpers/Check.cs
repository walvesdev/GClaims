using System.Diagnostics;
using GClaims.Core.Extensions;
using GClaims.Core.Filters.CustomExceptions;

namespace GClaims.Core.Helpers;

[DebuggerStepThrough]
public static class Check
{
    public static T NotNull<T>(T value, string errorMessage)
    {
        if (value == null)
        {
            throw new CustomException(errorMessage);
        }

        return value;
    }

    public static IList<T> NotNull<T>(IList<T> list, string errorMessage)
    {
        if (list == null || list.Count == 0)
        {
            throw new CustomException(errorMessage);
        }

        return list;
    }

    public static T NotNull<T>(T value, string parameterName, string message)
    {
        if (value == null)
        {
            throw new CustomException($"{parameterName} {message}");
        }

        return value;
    }

    public static string NotNull(string value, string parameterName, int maxLength = int.MaxValue, int minLength = 0)
    {
        if (value == null)
        {
            throw new CustomException(parameterName + " não pode ser nulo!");
        }

        if (value.Length > maxLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou inferior {maxLength}!");
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou maior que {minLength}!");
        }

        return value;
    }

    public static string NotNullOrWhiteSpace(string value, string parameterName, int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (value.IsNullOrWhiteSpace())
        {
            throw new CustomException(parameterName + " não pode ser nulo, vazio ou espaço em branco!");
        }

        if (value.Length > maxLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou inferior {maxLength}!");
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou maior que {minLength}!");
        }

        return value;
    }

    public static string NotNullOrEmpty(string value, string parameterName, int maxLength = int.MaxValue,
        int minLength = 0)
    {
        if (value.IsNullOrEmpty())
        {
            throw new CustomException(parameterName + " não pode ser nulo ou vazio!");
        }

        if (value.Length > maxLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou inferior {maxLength}!");
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou maior que {minLength}!");
        }

        return value;
    }

    public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string errorMessage)
    {
        if (value.IsNullOrEmpty())
        {
            throw new CustomException(errorMessage);
        }

        return value;
    }

    public static void EqualsCount<T>(ICollection<T> list1, ICollection<T> list2, string errorMessage)
    {
        if (!list1.IsNullOrEmpty() && !list2.IsNullOrEmpty() && list1.Count == list2.Count)
        {
            throw new CustomException(errorMessage);
        }
    }

    public static void NoEqualsCount<T>(ICollection<T> list1, ICollection<T> list2, string errorMessage)
    {
        if (!list1.IsNullOrEmpty() && !list2.IsNullOrEmpty() && list1.Count != list2.Count)
        {
            throw new CustomException(errorMessage);
        }
    }

    public static Type AssignableTo<TBaseType>(Type type, string parameterName)
    {
        NotNull(type, parameterName);
        if (!type.IsAssignableTo<TBaseType>())
        {
            throw new CustomException(parameterName + " (o tipo de " + type.AssemblyQualifiedName +
                                      ") deve ser atribuível ao " + typeof(TBaseType).GetFullNameWithAssemblyName() +
                                      "!");
        }

        return type;
    }

    public static string? Length(string? value, string parameterName, int maxLength, int minLength = 0)
    {
        if (minLength > 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new CustomException(parameterName + " não pode ser nulo ou vazio!");
            }

            if (value.Length < minLength)
            {
                throw new CustomException($"{parameterName} comprimento deve ser igual ou maior que {minLength}!");
            }
        }

        if (value != null && value.Length > maxLength)
        {
            throw new CustomException($"{parameterName} comprimento deve ser igual ou inferior {maxLength}!");
        }

        return value;
    }

    public static void DaysGreaterThanOrEqualTo(DateTime value, int totalDays, string errorMessage)
    {
        if (value == DateTime.MinValue || value == DateTime.MaxValue)
        {
            return;
        }

        if ((value - DateTime.Now).Days >= totalDays)
        {
            throw new CustomException(errorMessage);
        }
    }

    public static void DaysLessThanOrEqualTo(DateTime value, int totalDays, string errorMessage)
    {
        if (value == DateTime.MinValue || value == DateTime.MaxValue)
        {
            return;
        }

        if ((value - DateTime.Now).Days <= totalDays)
        {
            throw new CustomException(errorMessage);
        }
    }

    public static void Equals<T>(T value1, T value2, string errorMessage)
    {
        if (value1 != null && value1.IsNotNull() && value2.IsNotNull() && value1.Equals(value2))
        {
            throw new CustomException(errorMessage);
        }
    }

    public static void NotEquals<T>(T value1, T value2, string errorMessage)
    {
        if (value1 != null && value1.IsNotNull() && value2.IsNotNull() && !value1.Equals(value2))
        {
            throw new CustomException(errorMessage);
        }
    }

    public static bool IfTrueOrThrowExcepition(bool isTrue, string errorMessage, Exception? exception = null)
    {
        if (isTrue)
        {
            return isTrue;
        }

        if (exception.IsNotNull())
        {
            throw new CustomException(exception, errorMessage);
        }

        throw new CustomException(errorMessage);
    }
}