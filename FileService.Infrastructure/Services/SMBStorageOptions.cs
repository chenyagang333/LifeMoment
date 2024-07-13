using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure.Services
{
    public class SMBStorageOptions
    {
        /// <summary>
        /// 根目录
        /// </summary>
        ///千万不要写成private set；会导致不注入。项目里除了实体类、ValueObject、DTO之外，别的类尽量不要写成private set;
        public string WorkingDir { get; set; }
    }
}
