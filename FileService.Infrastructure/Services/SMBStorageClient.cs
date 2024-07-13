using FileService.Domain.IService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Services
{
    /// <summary>
    /// 局域网内共享文件夹或者本机磁盘当备份服务器的实现类
    /// </summary>
    public class SMBStorageClient : IStorageClient
    {
        public StorageType StorageType => StorageType.Backup;

        private readonly IOptionsSnapshot<SMBStorageOptions> optionsSnapshot;
        public SMBStorageClient(IOptionsSnapshot<SMBStorageOptions> optionsSnapshot)
        {
            this.optionsSnapshot = optionsSnapshot;
        }

        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith('/'))
            {
                throw new ArgumentException("key should not start with /", nameof(key));
            }
            string workingDir = optionsSnapshot.Value?.WorkingDir;
            string fullPath = Path.Combine(workingDir, key);
            string? fullDir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
            }
            using Stream stream = File.OpenWrite(fullPath);
            content.Position = 0;
            await content.CopyToAsync(stream, cancellationToken);
            //return new Uri(fullPath);
            return new Uri(key,UriKind.Relative); // 返回相对路径
        }
    }
}
