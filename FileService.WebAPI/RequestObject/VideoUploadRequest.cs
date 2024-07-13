using Chen.Commons.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FileService.WebAPI.RequestObject
{
    public class VideoUploadRequest
    {
        public ServiceType ServiceType { get; set; }
        public IFormFileCollection Files { get; set; }

        public class VideoUploadRequestValidator : AbstractValidator<VideoUploadRequest>
        {
            public VideoUploadRequestValidator()
            {
                //不用校验文件名的后缀，因为文件服务器会做好安全设置，所以即使用户上传exe、php等文件都是可以的
                long maxFileSize = 50 * 1024 * 1024;//最大文件大小
                RuleFor(e => e.Files).NotNull().Must(s => s.All(c => c.Length > 0 && c.Length < maxFileSize));
            }

        }
    }
}
