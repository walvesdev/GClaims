using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GClaims.Core.Filters.CustomExceptions;
using GClaims.Core.Helpers;

namespace GClaims.Core.Extensions;

public static class StringExtensions
{
    public static string TruncateIfNeeded(this string value, int maxlength)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length > maxlength)
        {
            return value[..maxlength];
        }

        return value;
    }

    /// <summary>
    /// Adiciona um caractere ao final de uma determinada string se não terminar com o caractere.
    /// </summary>
    public static string EnsureEndsWith(this string str, char c,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        Check.NotNull(str, "str");
        if (str.EndsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return str + c;
    }

    /// <summary>
    /// Adiciona um caractere ao início de uma determinada string se não começar com o caractere.
    /// </summary>
    public static string EnsureStartsWith(this string str, char c,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        Check.NotNull(str, "str");
        if (str.StartsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return c + str;
    }

    /// <summary>
    /// Indica se esta string é nula ou uma string System.String.Empty.
    /// </summary>
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// indica se esta string é nula, vazia ou consiste apenas em caracteres de espaço em branco.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// Obtém uma substring de uma string do início da string.
    /// </summary>
    public static string Left(this string str, int len)
    {
        Check.NotNull(str, "str");
        if (str.Length < len)
        {
            throw new CustomException("len argument can not be bigger than given string's length!");
        }

        return str[..len];
    }

    /// <summary>
    /// Converter terminações de linha na string para  <see cref="P:System.Environment.NewLine" />.
    /// </summary>
    public static string NormalizeLineEndings(this string str)
    {
        return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
    }

    /// <summary>Filtra somente os caracteres decimais na string</summary>
    /// <param name="source">Texto a ser filtrado</param>
    /// <returns></returns>
    public static string GetDigits(this string source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var stringBuilder = new StringBuilder();
        foreach (var c in source.Where(char.IsDigit))
        {
            stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Obtém o índice da enésima ocorrência de um char em uma string.
    /// </summary>
    /// <param name="str">string de origem a ser pesquisada</param>
    /// <param name="c">Char para pesquisar em <paramref name="str" /></param>
    /// <param name="n">Contagem da ocorrência</param>
    public static int NthIndexOf(this string str, char c, int n)
    {
        Check.NotNull(str, "str");
        var count = 0;
        for (var i = 0; i < str.Length; i++)
        {
            if (str[i] == c && ++count == n)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Remova a primeira ocorrência do postfix fornecido do final da string fornecida.
    /// </summary>
    /// <param name="str">a string.</param>
    /// <param name="postFixes">um ou mais postfix.</param>
    /// <returns>String modificada ou a mesma string se não tiver nenhum dos postfixes fornecidos</returns>
    public static string RemovePostFix(this string str, params string[] postFixes)
    {
        return str.RemovePostFix(StringComparison.Ordinal, postFixes);
    }

    /// <summary>
    /// Remova a primeira ocorrência do postfix fornecido do final da string fornecida.
    /// </summary>
    /// <param name="str">a string.</param>
    /// <param name="comparisonType">Tipo de comparação de string</param>
    /// <param name="postFixes">um ou mais postfix.</param>
    /// <returns>String modificada ou a mesma string se não tiver nenhum dos postfixes fornecidos</returns>
    public static string RemovePostFix(this string str, StringComparison comparisonType, params string[] postFixes)
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }

        if (postFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var postFix in postFixes)
        {
            if (str.EndsWith(postFix, comparisonType))
            {
                return str.Left(str.Length - postFix.Length);
            }
        }

        return str;
    }

    /// <summary>
    /// Remove a primeira ocorrência dos prefixos fornecidos do início da string fornecida.
    /// </summary>
    /// <param name="str">A string.</param>
    /// <param name="preFixes">um ou mais prefixo.</param>
    /// <returns>String modificada ou a mesma string se não tiver nenhum dos prefixos fornecidos</returns>
    public static string RemovePreFix(this string str, params string[] preFixes)
    {
        return str.RemovePreFix(StringComparison.Ordinal, preFixes);
    }

    /// <summary>
    /// Remove a primeira ocorrência dos prefixos fornecidos do início da string fornecida.
    /// </summary>
    /// <param name="str">A string.</param>
    /// <param name="comparisonType">Tipo de comparação de string</param>
    /// <param name="preFixes">um ou mais prefixo.</param>
    /// <returns>String modificada ou a mesma string se não tiver nenhum dos prefixos fornecidos</returns>
    public static string RemovePreFix(this string str, StringComparison comparisonType, params string[] preFixes)
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }

        if (preFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var preFix in preFixes)
        {
            if (str.StartsWith(preFix, comparisonType))
            {
                return str.Right(str.Length - preFix.Length);
            }
        }

        return str;
    }

    public static string ReplaceFirst(this string str, string search, string replace,
        StringComparison comparisonType = StringComparison.Ordinal)
    {
        Check.NotNull(str, "str");
        var pos = str.IndexOf(search, comparisonType);
        return pos < 0 ? str : string.Concat(str.AsSpan(0, pos), replace, str.AsSpan(pos + search.Length));
    }

    /// <summary>
    /// Obtém uma substring de uma string do final da string.
    /// </summary>
    public static string Right(this string str, int len)
    {
        Check.NotNull(str, "str");
        if (str.Length < len)
        {
            throw new CustomException("len argument can not be bigger than given string's length!");
        }

        return str.Substring(str.Length - len, len);
    }

    /// <summary>
    /// Usa o método string.Split para dividir determinada string por determinado separador.
    /// </summary>
    public static string[] Split(this string str, string separator)
    {
        return str.Split(new[] { separator }, StringSplitOptions.None);
    }

    /// <summary>
    /// Usa o método string.Split para dividir determinada string por determinado separador.
    /// </summary>
    public static string[] Split(this string str, string separator, StringSplitOptions options)
    {
        return str.Split(new[] { separator }, options);
    }

    /// <summary>
    /// Usa o método string.Split para dividir determinada string por <see cref="P:System.Environment.NewLine" />.
    /// </summary>
    public static string[] SplitToLines(this string str)
    {
        return Split(str, Environment.NewLine);
    }

    /// <summary>
    /// Usa o método string.Split para dividir determinada string por <see cref="P:System.Environment.NewLine" />.
    /// </summary>
    public static string[] SplitToLines(this string str, StringSplitOptions options)
    {
        return Split(str, Environment.NewLine, options);
    }

    /// <summary>
    /// Converte a string PascalCase para a string camelCase.
    /// </summary>
    /// <param name="str">String para converter</param>
    /// <param name="useCurrentCulture">
    /// definir true para usar a cultura atual. Caso contrário, a cultura invariável será
    /// usada.
    /// </param>
    /// <param name="handleAbbreviations">defina true para se você deseja converter 'XYZ' para 'xyz'.</param>
    /// <returns>camelCase da string</returns>
    public static string ToCamelCase(this string str, bool useCurrentCulture = false, bool handleAbbreviations = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return !useCurrentCulture ? str.ToLowerInvariant() : str.ToLower();
        }

        if (!handleAbbreviations || !IsAllUpperCase(str))
        {
            return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str.Substring(1);
        }

        return !useCurrentCulture ? str.ToLowerInvariant() : str.ToLower();
    }

    /// <summary>
    /// Converte a string PascalCase/camelCase em sentença (dividindo palavras por espaço).
    /// Exemplo: "ThisIsSampleSentence" é convertido em "Esta é uma frase de amostra".
    /// </summary>
    /// <param name="str">String para converter.</param>
    /// <param name="useCurrentCulture">
    /// definir true para usar a cultura atual. Caso contrário, a cultura invariável será
    /// usada.
    /// </param>
    public static string ToSentenceCase(this string str, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (!useCurrentCulture)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
        }

        return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
    }

    /// <summary>
    /// Converte a string PascalCase/camelCase para kebab-case.
    /// </summary>
    /// <param name="str">String para converter.</param>
    /// <param name="useCurrentCulture">
    /// definir true para usar a cultura atual. Caso contrário, a cultura invariável será
    /// usada.
    /// </param>
    public static string ToKebabCase(this string str, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        str = str.ToCamelCase();
        if (!useCurrentCulture)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLowerInvariant(m.Value[1]));
        }

        return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLower(m.Value[1]));
    }

    /// <summary>
    /// Converte a string PascalCase/camelCase dada para snake case.
    /// Exemplo: "ThisIsSampleSentence" é convertido em "this_is_a_sample_sentence".
    /// https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NameTranslation/NpgsqlSnakeCaseNameTranslator.cs#L51
    /// </summary>
    /// <param name="str">String para converter.</param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        var builder = new StringBuilder(str.Length + Math.Min(2, str.Length / 5));
        UnicodeCategory? previousCategory = null;
        for (var currentIndex = 0; currentIndex < str.Length; currentIndex++)
        {
            var currentChar = str[currentIndex];
            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            var currentCategory = char.GetUnicodeCategory(currentChar);
            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                        previousCategory == UnicodeCategory.LowercaseLetter ||
                        (previousCategory != UnicodeCategory.DecimalDigitNumber && previousCategory.HasValue &&
                         currentIndex > 0 && currentIndex + 1 < str.Length && char.IsLower(str[currentIndex + 1])))
                    {
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar);
                    break;
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append('_');
                    }

                    break;
                default:
                    if (previousCategory.HasValue)
                    {
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    }

                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converte string em valor enum.
    /// </summary>
    /// <typeparam name="T">Tipo de enum</typeparam>
    /// <param name="value">Valor da string a ser convertida</param>
    /// <returns>Retorna o objeto enum</returns>
    public static T ToEnum<T>(this string value) where T : struct
    {
        Check.NotNull(value, "value");
        return (T)Enum.Parse(typeof(T), value);
    }

    /// <summary>
    /// Converte string em valor enum.
    /// </summary>
    /// <typeparam name="T">Tipo de enum</typeparam>
    /// <param name="value">Valor da string a ser convertida</param>
    /// <param name="ignoreCase">Ignorar maiúsculas</param>
    /// <returns>Retorna o objeto enum</returns>
    public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
    {
        Check.NotNull(value, "value");
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static string ToMd5(this string str)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(str);
        var array = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        var array2 = array;
        foreach (var hashByte in array2)
        {
            sb.Append(hashByte.ToString("X2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converte a string camelCase em string PascalCase.
    /// </summary>
    /// <param name="str">String para converter</param>
    /// <param name="useCurrentCulture">
    /// definir true para usar a cultura atual. Caso contrário, a cultura invariável será
    /// usada.
    /// </param>
    /// <returns>PascalCase da string</returns>
    public static string ToPascalCase(this string str, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            if (!useCurrentCulture)
            {
                return str.ToUpperInvariant();
            }

            return str.ToUpper();
        }

        return (useCurrentCulture ? char.ToUpper(str[0]) : char.ToUpperInvariant(str[0])) + str.Substring(1);
    }

    /// <summary>
    /// Obtém uma substring de uma string do início da string se exceder o comprimento máximo.
    /// </summary>
    public static string Truncate(this string str, int maxLength)
    {
        if (str == null)
        {
            return null!;
        }

        return str.Length <= maxLength ? str : str.Left(maxLength);
    }

    /// <summary>
    /// Obtém uma substring de uma string de Ending da string se ela exceder o comprimento máximo.
    /// </summary>
    public static string TruncateFromBeginning(this string str, int maxLength)
    {
        if (str == null)
        {
            return null!;
        }

        return str.Length <= maxLength ? str : str.Right(maxLength);
    }

    /// <summary>
    /// Obtém uma substring de uma string do início da string se exceder o comprimento máximo.
    /// Adiciona um postfix "..." ao final da string se ela estiver truncada.
    /// A string de retorno não pode ser maior que maxLength.
    /// </summary>
    public static string TruncateWithPostfix(this string str, int maxLength)
    {
        return str.TruncateWithPostfix(maxLength, "...");
    }

    /// <summary>
    /// Obtém uma substring de uma string do início da string se exceder o comprimento máximo.
    /// Adiciona dado <paramref name="postfix" /> ao final da string se ela estiver truncada.
    /// A string de retorno não pode ser maior que maxLength.
    /// </summary>
    public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
    {
        if (str == null)
        {
            return null!;
        }

        if (str == string.Empty || maxLength == 0)
        {
            return string.Empty;
        }

        if (str.Length <= maxLength)
        {
            return str;
        }

        if (maxLength <= postfix.Length)
        {
            return postfix.Left(maxLength);
        }

        return str.Left(maxLength - postfix.Length) + postfix;
    }

    /// <summary>
    /// Converte determinada string em uma matriz de bytes usando <see cref="P:System.Text.Encoding.UTF8" /> encoding.
    /// </summary>
    public static byte[] GetBytes(this string str)
    {
        return str.GetBytes(Encoding.UTF8);
    }

    /// <summary>
    /// Converte determinada string em uma matriz de bytes usando o dado <paramref name="encoding" />
    /// </summary>
    public static byte[] GetBytes(this string str, Encoding encoding)
    {
        Check.NotNull(str, "str");
        Check.NotNull(encoding, "encoding");
        return encoding.GetBytes(str);
    }

    /// <summary>
    /// Converte um byte[] em string sem BOM (A marca de ordem de byte (BOM)
    /// é um caractere Unicode usado para denotar a extremidade (ordem de bytes)
    /// de um arquivo de texto ou fluxo de dados, cujo código é U+FEFF. Seu uso é opcional e,
    /// se usado, deve aparecer no começo do fluxo de texto.).
    /// </summary>
    /// <param name="bytes">O byte[] a ser convertido em string</param>
    /// <param name="encoding">A codificação para obter a string. O padrão é UTF8</param>
    /// <returns></returns>
    public static string ConvertFromBytesWithoutBom(byte[] bytes, Encoding? encoding = null)
    {
        if (bytes == null)
        {
            return null!;
        }

        if (encoding == null)
        {
            encoding = Encoding.UTF8;
        }

        if (bytes.Length >= 3 && bytes[0] == 239 && bytes[1] == 187 && bytes[2] == 191)
        {
            return encoding.GetString(bytes, 3, bytes.Length - 3);
        }

        return encoding.GetString(bytes);
    }

    private static bool IsAllUpperCase(string input)
    {
        return input.All(t => !char.IsLetter(t) || char.IsUpper(t));
    }

    public static string RemoveScape(this string text)
    {
        if (text.IsNullOrWhiteSpace())
        {
            return text;
        }

        return text
            .Replace("  ", " ")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\"", "");
    }
}