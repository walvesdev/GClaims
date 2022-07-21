using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace GClaims.Core.Attributes.Validation;

public class ListValidateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is IList list)
        {
            return list.Count > 0;
        }

        return false;
    }
}