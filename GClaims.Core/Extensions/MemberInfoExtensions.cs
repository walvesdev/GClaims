using System.Reflection;
using GClaims.Core.Filters.CustomExceptions;

namespace GClaims.Core.Extensions;

/// <summary>
/// Extensões para <see cref="T:System.Reflection.MemberInfo" />.
/// </summary>
public static class MemberInfoExtensions
{
    /// <summary>
    /// Obtém um único atributo para um membro.
    /// </summary>
    /// <typeparam name="TAttribute">Tipo do atributo</typeparam>
    /// <param name="memberInfo">O membro que será verificado quanto ao atributo</param>
    /// <param name="inherit">Incluir atributos herdados</param>
    /// <returns>Retorna o objeto de atributo se encontrado. Retorna null se não for encontrado.</returns>
    public static TAttribute? GetSingleAttributeOrNull<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
        where TAttribute : Attribute
    {
        if (memberInfo == null)
        {
            throw new CustomException("memberInfo");
        }

        var attrs = memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).ToArray();
        if (attrs.Length != 0)
        {
            return (TAttribute)attrs[0];
        }

        return null;
    }

    public static TAttribute? GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(this Type type, bool inherit = true)
        where TAttribute : Attribute
    {
        while (true)
        {
            var attr = type?.GetTypeInfo().GetSingleAttributeOrNull<TAttribute>();
            if (attr != null)
            {
                return attr;
            }

            if (type != null && type.GetTypeInfo().BaseType == null)
            {
                return null;
            }

            type = type?.GetTypeInfo()?.BaseType;
        }
    }
}