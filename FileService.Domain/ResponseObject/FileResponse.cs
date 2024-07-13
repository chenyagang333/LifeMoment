using Chen.Commons.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.ResponseObject
{
    public class FileResponse
    {
        public FileType Type { get; set; }
        public List<string> Urls { get; set; }
    }
}
