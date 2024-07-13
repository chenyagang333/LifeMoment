using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public class FSRespository : IFSRespository
    {
        private readonly FSDbContext ctx;

        public FSRespository(FSDbContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash)
        {
            return await ctx.UploadedItems.FirstOrDefaultAsync(x => x.FileSizeInBytes == fileSize &&
            x.FileSHA256Hash == sha256Hash);
        }
    }
}
