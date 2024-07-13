using Chen.DomainCommons.Models;

namespace FileService.Domain.Entities
{
    public record UploadedItem : BaseEntity, IHasCreateTime
    {
        public DateTime CreateTime { get; private set; }
        /// <summary>
        /// 文件大小（尺寸为字节）
        /// </summary>
        public long FileSizeInBytes { get; private set; }
        /// <summary>
        /// 用户上传的原始文件名（没有路径）
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// 两个文件的大小和散列值（SHA256）都相同的概率非常小。因此只要大小和SHA256相同，就认为是相同的文件。
        /// SHA256的碰撞概率比MD5要小很多
        /// </summary>
        public string FileSHA256Hash { get; private set; }
        /// <summary>
        /// 备份文件路径，因为可能会更换文件储存系统或云储存供应商，因此系统会保存一份自有的路径。
        /// 备份文件一般放在内网的高速、稳定的设备上，比如 NAS 等。
        /// </summary>
        public Uri BackUrl { get; private set; }        
        /// <summary>
        /// 上传的文件供外部访问者访问的路径
        /// </summary>
        public Uri RemoteUrl { get; private set; }

        public static UploadedItem Create(long fileSizeInBytes,string fileName ,
            string fileSHA256Hash,Uri backupUrl,Uri remoteUrl)
        {
            return new UploadedItem
            {
                CreateTime = DateTime.Now,
                FileName = fileName,
                FileSHA256Hash = fileSHA256Hash,
                FileSizeInBytes = fileSizeInBytes,
                BackUrl = backupUrl,
                RemoteUrl = remoteUrl
            };
        }

    }
}
