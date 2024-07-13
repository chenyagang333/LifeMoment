using FluentValidation;
using IdentityService.Domain.ValueObjects;

namespace IdentityService.WebAPI.RequestObject.User
{
    public record CreateUserByPhoneNumber(
        string UserName,
        string PhoneNumber,
        //PhoneNumber PhoneNumber,
        string Password,
        string RightPassword,
        string Code
        );

    public class CreateUserByPhoneNumberRequestValidator : AbstractValidator<CreateUserByPhoneNumber>
    {
        public CreateUserByPhoneNumberRequestValidator()
        {
            RuleFor(e => e.Password).Equal(e => e.RightPassword)
                .WithMessage("Password and RightPassword must be equal."); // 设置验证失败时的自定义错误消息;
            RuleFor(e => e.UserName).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(e => e.PhoneNumber).MinimumLength(11).MaximumLength(11)
                .NotNull().NotEmpty(); // 设置验证失败时的自定义错误消息;
            RuleFor(e => e.Code).NotNull().NotEmpty().WithMessage("手机号验证码不能为空！")
                .MaximumLength(6).MinimumLength(6).WithMessage("手机号验证码格式有误！");
        }
    }
}
