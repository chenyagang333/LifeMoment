using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons
{
    public static class IOHelper
    {
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
        {
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            return ms.ToArray();
        }

        public static void CreateDir(FileInfo file)
        {
            file.Directory.Create();
        }
    }
}
