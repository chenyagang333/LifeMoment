using Chen.Commons.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Utils
{
    public static class DomainUtils
    {

        public static string GetServiceNameByType(this string ServiceType)
        {
            switch (ServiceType)
            {
                case "0":
                    return "YouShow";
                case "1":
                    return "Identity";
                case "2":
                    return "Comment";
                default:
                    return "Other";
            }

        }

        public static FileType GetFileTypeByFileName(this string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
            {
                // 扩展名为空，无法确定文件类型
                return FileType.Unknown;
            }

            // 图片文件扩展名列表
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };
            // 视频文件扩展名列表
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".webm" };

            // 检查扩展名是否匹配图片文件或视频文件
            if (Array.Exists(imageExtensions, ext => ext == extension))
            {   
                return FileType.Image;
            }
            else if (Array.Exists(videoExtensions, ext => ext == extension))
            {
                return FileType.Video;
            }
            else
            {
                // 其他扩展名，无法确定文件类型
                return FileType.Unknown;
            }
        }

    }
}
