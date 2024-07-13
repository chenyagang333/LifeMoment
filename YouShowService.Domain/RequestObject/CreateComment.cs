using FluentValidation;
namespace YouShowService.Domain.RequestObject
{
    public record CreateComment(
        List<string>? ImageURLList,
        string UserName,
        string? UserAvatarURL,
        string? PublishAddress,
        string? Content,
        long ShowId
        );

    public class CreateCommentRequestValidator : AbstractValidator<CreateComment>
    {
        public CreateCommentRequestValidator()
        {
            RuleFor(e => e.UserName).NotNull().NotEmpty();
            RuleFor(e => e).Must(x => !string.IsNullOrWhiteSpace(x.Content)
            || x.ImageURLList != null).WithMessage("缺少要发布的内容！");
            RuleFor(e => e.ShowId).NotNull().NotEmpty();
        }
    }
}

