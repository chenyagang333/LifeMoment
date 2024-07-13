using FluentValidation;

namespace IdentityService.WebAPI.RequestObject.Login
{
    public record LoginDTO(string Account,string Password);

    public class LoginRequestValidator : AbstractValidator<LoginDTO>
    {
        public LoginRequestValidator()
        {
            RuleFor(e => e.Account).NotNull().NotEmpty();
            RuleFor(e => e.Password).NotNull().NotEmpty();
        }
    }
}

