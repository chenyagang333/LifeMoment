using Microsoft.AspNetCore.Http;

namespace FileService.Domain.RequestObject
{
    public class FilesReques
    {
        public string ServiceType { get; set; }
        public List<FileData> FileData { get; set; }
    }
    public class FileData
    {
        public IFormFile FirstFile { get; set; }
        public IFormFile? SecondFile { get; set; }
    }
}
