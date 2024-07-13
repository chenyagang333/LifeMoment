using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FileService.WebAPI.RequestObject
{
    public class UploadRequest123
    {
        public IFormFile File { get; set; }
        public string ServiceType { get; set; }
        //public IFormFileCollection File;
    }

    public class UploadRequestValidator : AbstractValidator<UploadRequest123>
    {
        public UploadRequestValidator()
        {
            //不用校验文件名的后缀，因为文件服务器会做好安全设置，所以即使用户上传exe、php等文件都是可以的
            long maxFileSize = 50 * 1024 * 1024;//最大文件大小
                                                //RuleFor(e => e.File).NotNull().Must(f => f.Length > 0 && f.Length < maxFileSize);
            RuleFor(e => e.ServiceType).NotNull().NotEmpty().Must(BeValidServiceType);
        }
        private bool BeValidServiceType(string serviceType)
        {
            if (!int.TryParse(serviceType, out int value))
            {
                // 如果无法解析为整数，则认为不是有效的 ServiceType
                return false;
            }

            // 检查 ServiceType 是否在 0 到 10 之间
            return value >= 0 && value <= 10;
        }
    }
}
