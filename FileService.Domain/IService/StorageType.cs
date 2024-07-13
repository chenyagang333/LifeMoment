using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.IService
{
    public enum StorageType
    {
        Public, // 供公共访问的设备
        Backup // 内网备份用的存储设备
    }
}
