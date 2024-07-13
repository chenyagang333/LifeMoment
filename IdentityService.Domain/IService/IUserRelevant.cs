using Chen.Commons.FunResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.IService
{
    public interface IUserRelevant
    {
        Task<bool> GetSignInStateAsync(long userId);
        Task<FunResult> SignInAsync(long userId);
        Task<int> GetCountOfSignInForMonthAsync(long userId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">我</param>
        /// <param name="toUserId">TA</param>
        /// <returns>00：谁也没关注对方，10：代表我关注TA，01：代表TA关注我，11：代表互相关注</returns>
        Task<string> GetAttentionStateAsync(long userId,long toUserId);
        /// <summary>
        /// 用户新增关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        Task UserAddAttentionAsync(long userId, long toUserId);

        /// <summary>
        /// 用户取消关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        Task UserCancelAttentionAsync(long userId, long toUserId);

    }
}
