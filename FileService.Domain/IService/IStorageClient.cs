using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.IService
{
    public interface IStorageClient
    {
        StorageType StorageType { get; }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="key">文件的 Key （一般是文件路径的一部分）</param>
        /// <param name="content">文件内容</param>
        /// <returns>返回可以被访问的文件 Uri</returns>
        Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default);
    }
}
