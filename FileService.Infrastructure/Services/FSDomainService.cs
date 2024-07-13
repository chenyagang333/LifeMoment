using Chen.Commons;
using FileService.Domain;
using FileService.Domain.Entities;
using FileService.Domain.IService;
using FileService.Domain.RequestObject;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileService.Domain.Utils;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;
using FileService.Domain.ResponseObject;
using Chen.Commons.FileUtils;
using Microsoft.AspNetCore.Hosting;
using Chen.Commons.Common;

namespace FileService.Infrastructure.Services
{
    public class FSDomainService : IFSDomainService
    {
        private readonly IFSRespository fSRespository;
        private readonly IStorageClient backupStorage; // 备份服务器
        private readonly IStorageClient remoteStorage; // 文件储存服务器
        private readonly FSDbContext ctx; // 
        private readonly IWebHostEnvironment webHostEnvironment;

        public FSDomainService(IFSRespository fSRespository, IEnumerable<IStorageClient> storageClients, FSDbContext ctx, IWebHostEnvironment webHostEnvironment)
        {
            this.fSRespository = fSRespository;
            // 用这种方式可以解决内置 DI 不能使用名字注入不同实例的问题，而且从原则上讲更优美
            this.backupStorage = storageClients.First(x => x.StorageType == StorageType.Backup);
            this.remoteStorage = storageClients.First(x => x.StorageType == StorageType.Public);
            this.ctx = ctx;
            this.webHostEnvironment = webHostEnvironment;
        }
        // 领域服务只有抽象的业务逻辑 
        public async Task<IEnumerable<MyFileInfo>?> UploadAsync(string serviceType, IFormFileCollection formFiles, CancellationToken cancellationToken)
        {
            List<MyFileInfo>? list = [];
            var serviceName = serviceType.GetServiceNameByType();
            int count = 0;
            while (count < formFiles.Count)
            {
                var file = formFiles[count];
                var fileType = file.FileName.GetFileTypeByFileName();
                var url = await UploadFilesAsync(serviceName, file, fileType, cancellationToken);
                MyFileInfo data = new MyFileInfo { Type = fileType, FirstURL = url };
                if (fileType == FileType.Video)
                {
                    // 视频的封面图
                    var imageFile = formFiles[count + 1];
                    var imageUrl = await UploadFilesAsync(serviceName, imageFile, FileType.Image, cancellationToken);
                    data.SecondURL = imageUrl;
                    count += 2;
                }
                else { count++; }
                list.Add(data);
            }
            return list;
        }

        // 
        public async Task<string> UploadFilesAsync(string serviceName, IFormFile file, FileType fileType, CancellationToken cancellationToken)
        {

            DateTime today = DateTime.Today;
            var baseKey = $"{serviceName}/{fileType.ToStringName()}/{today.Year}/{today.Month}/{today.Day}";
            var url = await UploadFileAsync(baseKey, file, cancellationToken);
            return url;
        }

        // 保存文件 返回存储路径
        public async Task<string> UploadFileAsync(string baseKey, IFormFile file, CancellationToken cancellationToken)
        {
            var stream = file.OpenReadStream();
            var hash = HashHelper.ComputeSha256Hash(stream);
            var fileSize = stream.Length;
            var fileName = file.FileName;
            var key = $"{baseKey}/{hash}/{fileName}"; // 拼接完整的存储路径
            // 如果已经上传过，则直接返回
            // 查询是否有和上传文件的大小和 SHA256 一样的文件，如果有的话，就认为是同一个文件
            // 虽然说前端可能已经调用 FileExists 接口检查过了，但是前端可能跳过了，或者有并发上传等问题，所以这里再检查一遍
            var oldUploadItem = await fSRespository.FindFileAsync(fileSize, hash);
            // 判断记录是否存在，以及文件是否存在 》》文件存储在项目根目录下的wwwroot下
            if (oldUploadItem != null)
            {
                if (File.Exists(Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot", key)))
                {
                    return oldUploadItem.RemoteUrl.OriginalString; // 文件也存在则返回url
                }
                else
                {
                    ctx.UploadedItems.Remove(oldUploadItem); // 文件不存在则删除记录
                }
            }
            //Uri backupUrl = await backupStorage.SaveAsync(key, stream, cancellationToken); // 保存本地备份
            //stream.Position = 0;
            Uri remoteUrl = await remoteStorage.SaveAsync(key, stream, cancellationToken); // 保存到生产的储存系统
            stream.Position = 0;
            // 领域服务并不会真正的执行数据库插入，只是把实体生成，然后有应用服务和基础设施来配合完成插入数据库。
            // DDD 中尽量避免直接在领域服务中执行数据库的修改（包含删除、新增）操作。
            //var data = UploadedItem.Create(fileSize, fileName, hash, backupUrl, remoteUrl);
            var data = UploadedItem.Create(fileSize, fileName, hash, remoteUrl, remoteUrl);
            // 在这里操作一下实体，更方便一些
            await ctx.UploadedItems.AddAsync(data);
            return data.RemoteUrl.OriginalString;
        }


        // 上传图片文件
        public async Task<MyFileInfo> ImageUploadAsync(ServiceType serviceType, IFormFile file, CancellationToken cancellationToken)
        {
            var baseKey = GetBaseKey(serviceType, file);
            var url = await UploadFileAsync(baseKey, file, cancellationToken);
            MyFileInfo data = new MyFileInfo { Type = FileType.Image, FirstURL = url };
            return data;
        }

        // 上传视频文件（包含封面图片文件）
        public async Task<MyFileInfo> VideoUploadAsync(ServiceType serviceType, IFormFileCollection files, CancellationToken cancellationToken)
        {
            var videoBaseKey = GetBaseKey(serviceType, files[0]); // 拿到视频存储路径
            var coverBaseKey = GetBaseKey(serviceType, files[1]); // 拿到视频封面存储路径
            var videoUrl = await UploadFileAsync(videoBaseKey, files[0], cancellationToken); // 存储视频文件
            var coverUrl = await UploadFileAsync(coverBaseKey, files[1], cancellationToken); // 存储视频封面图片文件
            return new MyFileInfo { Type = FileType.Video, FirstURL = videoUrl, SecondURL = coverUrl };
        }

        // 获取文件的基本存储路径
        private string GetBaseKey(ServiceType serviceType, IFormFile file)
        {
            DateTime today = DateTime.Today;
            var serviceName = serviceType.ToString();
            var fileTypeName = file.FileName.GetFileTypeByFileName().ToStringName();
            var baseKey = $"{serviceName}/{fileTypeName}/{today.Year}/{today.Month}/{today.Day}";
            return baseKey;
        }

    }
}
