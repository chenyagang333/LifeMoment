using FileService.Domain.RequestObject;
using FluentValidation;

namespace FileService.WebAPI.RequestObject
{
    //public class FileRequestValidator : AbstractValidator<FileRequest>
    //{
    //    public FileRequestValidator()
    //    {
    //        //不用校验文件名的后缀，因为文件服务器会做好安全设置，所以即使用户上传exe、php等文件都是可以的
    //        long maxFileSize = 50 * 1024 * 1024;//最大文件大小
    //        RuleFor(e => e.FileData)
    //            .Must(x => x != null ? x.All(c =>
    //            c.FirstFile.Length > 0 &&
    //            c.FirstFile.Length < maxFileSize &&
    //            c.SecondFile != null ?
    //            (c.SecondFile?.Length > 0 &&
    //            c.SecondFile?.Length < maxFileSize * 10) : true
    //            ) : true).WithMessage("文件大小超出范围");
    //        RuleFor(e => e.ServiceType).NotNull().NotEmpty().Must(BeValidServiceType);
    //    }
    //    private bool BeValidServiceType(string serviceType)
    //    {
    //        if (!int.TryParse(serviceType, out int value))
    //        {
    //            // 如果无法解析为整数，则认为不是有效的 ServiceType
    //            return false;
    //        }

    //        // 检查 ServiceType 是否在 0 到 10 之间
    //        return value >= 0 && value <= 10;
    //    }
    //}
}
