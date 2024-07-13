using Chen.Commons;
using FluentValidation;
using System.Text.RegularExpressions;

namespace IdentityService.WebAPI.RequestObject.Login
{
    public record LoginByMailDTO(string Mail,string Code);

    public class LoginByMailDTORequestValidator : AbstractValidator<LoginByMailDTO>
    {
        public LoginByMailDTORequestValidator()
        {
            RuleFor(e => e.Mail).NotNull().NotEmpty().Matches(new Regex(RegexHelper.MailRegex)).WithMessage("邮箱无效");
            RuleFor(e => e.Code).NotNull().NotEmpty();
        }
    }
}
