using FluentValidation;

namespace YouShowService.Domain.RequestObject
{
    public record CreateReply(
        List<string>? ImageURLList,
        string UserName,
        string ToUserName,
        string? UserAvatarURL,
        string? PublishAddress,
        string? Content,
        long CommentId,
        long ShowId
        );

    public class CreateReplyRequestValidator : AbstractValidator<CreateReply>
    {
        public CreateReplyRequestValidator()
        {
            RuleFor(e => e.UserName).NotNull().NotEmpty();
            RuleFor(e => e).Must(x => !string.IsNullOrWhiteSpace(x.Content)
            || x.ImageURLList != null).WithMessage("缺少要发布的内容！");
            RuleFor(e => e.CommentId).NotNull().NotEmpty();
        }
    }
}


