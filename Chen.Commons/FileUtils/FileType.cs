using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons.FileUtils
{
    public enum FileType
    {
        Unknown = 0,
        Image = 1,
        Video = 2,
    }

    public static class HandleFileType
    {
        public static string ToStringName(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Image:
                    return "Images";
                case FileType.Video:
                    return "Videos";
            }
            return "Unknowns";
        }
    }
}
