using Chen.Commons.FileUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.Entities
{
    public record YouShowFile:BaseFile
    {
        public long YouShowId { get; set; }

        public YouShowFile UpdateYouShowId(long youShowId)
        {
            YouShowId = youShowId;
            return this;
        }
    }
}
