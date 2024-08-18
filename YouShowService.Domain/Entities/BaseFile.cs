using Chen.Commons.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Entities
{
    public record BaseFile
    {
        public long Id { get; set; }
        public int sort { get; set; }
        public FileType Type { get; set; }
        public string FirstURL { get; set; }
        public string? SecondURL { get; set; }

        public BaseFile SpliceFirstURL(string baseUrl)
        {
            if (!string.IsNullOrEmpty(FirstURL))
            {
                FirstURL = baseUrl + FirstURL;
            }
            return this;
        }
        public BaseFile SpliceSecondURL(string baseUrl)
        {
            if (!string.IsNullOrEmpty(SecondURL))
            {
                SecondURL = baseUrl + SecondURL;
            }
            return this;
        }
    }
}
