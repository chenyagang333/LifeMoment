using Chen.Commons.Common;
using Chen.Commons.FileUtils;
using FileService.Domain.Entities;
using FileService.Domain.RequestObject;
using FileService.Domain.ResponseObject;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.IService
{
    public interface IFSDomainService
    {
        Task<IEnumerable<MyFileInfo>?> UploadAsync(string serviceType, IFormFileCollection formFiles, CancellationToken cancellationToken);
        Task<string> UploadFilesAsync(string serviceName, IFormFile file, FileType fileType, CancellationToken cancellationToken);
        Task<string> UploadFileAsync(string baseKey, IFormFile file, CancellationToken cancellationToken);
        Task<MyFileInfo> ImageUploadAsync(ServiceType serviceType, IFormFile file, CancellationToken cancellationToken);
        Task<MyFileInfo> VideoUploadAsync(ServiceType serviceType, IFormFileCollection files, CancellationToken cancellationToken);
    }
}
