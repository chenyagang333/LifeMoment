using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.ValueObjects
{
    /// <summary>
    /// 手机号码
    /// </summary>
    /// <param name="RegionNumber">手机号码区域号</param>
    /// <param name="Number">手机号码</param>
    public record PhoneNumber(int RegionNumber, string Number);
}
