using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation;
public static class UriValidators
{
    public static IRuleBuilderOptions<T,Uri> NotEmptyUri<T>(this IRuleBuilder<T,Uri> ruleBuilder)
    {
        return ruleBuilder.Must(p => !string.IsNullOrEmpty(p.OriginalString) && !string.IsNullOrWhiteSpace(p.OriginalString))
            .WithMessage("The Uri nust not be null nor empty");
    }

    public static IRuleBuilderOptions<T,Uri> Length<T>(this IRuleBuilder<T,Uri> ruleBuilder, int min,int max)
    {
        // 为空则跳过检查，因为有专门的 NotEmptyUri 判断，也许一个东西允空，但是不为空的时候再检查
        return ruleBuilder.Must(p => string.IsNullOrWhiteSpace(p.OriginalString) ||
        (p.OriginalString.Length >= min && p.OriginalString.Length <= max))
            .WithMessage($"The length of Uri must be between {min} and {max}.");
    }
}
