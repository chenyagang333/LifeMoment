using FileService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain
{
    public interface IFSRespository
    {
        Task<UploadedItem?> FindFileAsync(long fileSize,string sha256Hash); 
    }
}
