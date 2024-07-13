using Chen.Commons.FileUtils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using YouShowService.Domain.Entities;

namespace YouShowService.Domain.RequestObject
{
    public record CreateYouShow(
        string UserName,
        string? UserAvatarURL,
        string? PublishAddress,
        string? Content,
        List<YouShowFile>? Files
        //IFormFileCollection? FileCollection
        );

    public class CreateYouShowRequestValidator : AbstractValidator<CreateYouShow>
    {
        public CreateYouShowRequestValidator()
        {
            RuleFor(e => e.UserName).NotNull().NotEmpty();
            RuleFor(e => e.UserAvatarURL).NotNull().NotEmpty();
            RuleFor(e => e.Content).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("缺少要发布的内容！");
        }
    }
}
