using FluentValidation;

namespace IdentityService.WebAPI.RequestObject.User
{
    public class EditUserData
    {
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string Description { get; set; }
    }
    public class EditUserDataValidator : AbstractValidator<EditUserData>
    {
        public EditUserDataValidator()
        {
            RuleFor(e => e.UserName).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(e => e.UserAvatar).NotNull().NotEmpty(); // ;
        }
    }
}
