using System.ComponentModel.DataAnnotations;

namespace Web;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class CantEvenAttribute: ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return false;
    }
}
