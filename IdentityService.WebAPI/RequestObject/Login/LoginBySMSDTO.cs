using Chen.Commons;
using FluentValidation;
using System.Text.RegularExpressions;

namespace IdentityService.WebAPI.RequestObject.Login
{
    public record LoginBySMSDTO(string PhoneNumber,string Code);

    public class LoginBySMSRequestValidator : AbstractValidator<LoginBySMSDTO>
    {
        public LoginBySMSRequestValidator()
        {
            RuleFor(e => e.PhoneNumber).NotNull().NotEmpty().Matches(new Regex(RegexHelper.PhoneNumberRegex)).WithMessage("电话号码无效");
            RuleFor(e => e.Code).NotNull().NotEmpty();
        }
    }
}
