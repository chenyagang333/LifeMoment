using FileService.Domain.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Services
{
    public class MockCloudStorageClient : IStorageClient
    {
        public StorageType StorageType => StorageType.Public;

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        public MockCloudStorageClient(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith('/'))
            {
                throw new ArgumentException("key should not start with /", nameof(key));
            }
            string workingDir = Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot");
            string fullPath = Path.Combine(workingDir, key);
            string? fullName = Path.GetDirectoryName(fullPath);
            Directory.CreateDirectory(fullName);

            using Stream outStream = File.OpenWrite(fullPath);
            content.Position = 0;
            await content.CopyToAsync(outStream,cancellationToken);
            var req = httpContextAccessor.HttpContext.Request;
            // 储存绝对路径
            //string url = $"{req.Scheme}://{req.Host}/FileService/{key}";
            //return new Uri(url, UriKind.Absolute);
            // 储存相对路径
            return new Uri(key,UriKind.Relative);
        }
    }
}
