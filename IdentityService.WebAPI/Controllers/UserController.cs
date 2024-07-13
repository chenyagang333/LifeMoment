using AutoMapper;
using Chen.ASPNETCore;
using Chen.Commons;
using Chen.Commons.ApiResult;
using IdentityService.Domain.DTO;
using IdentityService.Domain.Entities;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
using IdentityService.Domain.Notifications;
using IdentityService.Infrastructure;
using IdentityService.WebAPI.RequestObject.User;
using IdentityService.WebAPI.ResponseObject.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Transactions;

namespace IdentityService.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRespository userRespository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRelevant userRelevant;
        private readonly UserDbContext userDbContext;
        private readonly IMapper mapper;
        private readonly IMediator _mediator;

        public UserController(
            IUserRespository userRespository,
            IHttpContextAccessor httpContextAccessor,
            IUserRelevant userRelevant,
            UserDbContext userDbContext,
            IMapper mapper
,
            IMediator mediator)
        {
            this.userRespository = userRespository;
            this.httpContextAccessor = httpContextAccessor;
            this.userRelevant = userRelevant;
            this.userDbContext = userDbContext;
            this.mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetUserDataByToken()
        {
            var addressIp = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            string? userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdStr != null && long.TryParse(userIdStr, out long userId))
            {
                User? user = await userRespository.FindByUserIdAsync(userId);
                if (user != null)
                {
                    var userDTO = mapper.Map<User, UserDTO>(user);
                    return ApiResult.Succeeded(userDTO);
                }
            }
            return ApiResult.Failed("未找到用户信息");
        }
        /// <summary>
        /// 根据用户Id获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult>> GetUserDataById(long userId)
        {
            User? user = await userRespository.FindByUserIdAsync(userId);
            if (user == null) return ApiResult.Failed("未找到用户信息");
            var userDTO = mapper.Map<User, UserDTO>(user);
            return ApiResult.Succeeded(userDTO);
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(typeof(UserDbContext))]
        [HttpPost]
        public async Task<ActionResult<ApiResult>> EditUserData(EditUserData editUserData)
        {
            var userId = HttpHelper.TryGetUserId(HttpContext);
            if (userId > 0)
            {
                User? user = await userRespository.FindByUserIdAsync(userId);
                if (user == null) return ApiResult.Failed("未找到用户信息");
                user.UpdateUserName(editUserData.UserName);
                user.ChangeUserAvatar(editUserData.UserAvatar);
                user.UpdateDescription(editUserData.Description);
                await userDbContext.SaveChangesAsync();
                // 领域实体保存成功后，发布领域事件
                await _mediator.DispatchDomainEventsAsync(userDbContext);
            }
            return ApiResult.Succeeded("修改成功！");
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        [UnitOfWork(typeof(UserDbContext))]
        [HttpGet]
        public async Task<ActionResult<ApiResult>> SignIn(long userId)
        {
            var res = await userRelevant.SignInAsync(userId);
            if (res.Succeeded)
            {
                return ApiResult.Succeeded("签到成功！");
            }
            return ApiResult.Failed(res.Description);
        }

        ///// <summary>
        ///// 获取用户的签到状态
        ///// </summary>
        ///// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<bool>> GetSignInState(long userId)
        {
            var state = await userRelevant.GetSignInStateAsync(userId);
            return state;

        }
        ///// <summary>
        ///// 获取用户本月的签到次数
        ///// </summary>
        ///// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<int>> GetCountOfSignInForMonth(long userId)
        {
            var count = await userRelevant.GetCountOfSignInForMonthAsync(userId);
            return count;

        }

        /// <summary>
        /// 获取关注状态—— 0：谁也没关注对方，1：代表我关注TA，2：代表TA关注我，3：代表互相关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> GetAttentionState(long userId, long toUserId)
        {
            var state = await userRelevant.GetAttentionStateAsync(userId, toUserId);
            return ApiResult.Succeeded(state);
        }

        /// <summary>
        /// 点关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> UserAddAttention(long userId, long toUserId)
        {
            await userRelevant.UserAddAttentionAsync(userId, toUserId);
            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
            await userDbContext.SaveChangesAsync();
            transactionScope.Complete();
            // 领域实体保存成功后，发布领域事件
            await _mediator.DispatchDomainEventsAsync(userDbContext);
            return ApiResult.Succeess;
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResult>> UserCancelAttention(long userId, long toUserId)
        {
            await userRelevant.UserCancelAttentionAsync(userId, toUserId);
            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
            await userDbContext.SaveChangesAsync();
            transactionScope.Complete();
            // 领域实体保存成功后，发布领域事件
            await _mediator.DispatchDomainEventsAsync(userDbContext);
            return ApiResult.Succeess;
        }




//        [UnitOfWork(typeof(UserDbContext))]
//        [AllowAnonymous]
//        [HttpGet]
//        public async void dasdasdasd(long userId, long toUserId)
//        {
//			string ss = @"<p style=""text-align: center;"">
//	1、今宵别梦寒</p>
//<p style=""text-align: center;"">
//	2、有你的未来</p>
//<p style=""text-align: center;"">
//	3、清轩挽风长</p>
//<p style=""text-align: center;"">
//	4、吹一样的风</p>
//<p style=""text-align: center;"">
//	5、痛也没关系</p>
//<p style=""text-align: center;"">
//	6、沽酒待人归</p>
//<p style=""text-align: center;"">
//	7、一曲红尘醉</p>
//<p style=""text-align: center;"">
//	8、咸咸的海风</p>
//<p style=""text-align: center;"">
//	9、久溯的回光</p>
//<p style=""text-align: center;"">
//	10、草莓棉花糖</p>
//<p style=""text-align: center;"">
//	11、啾一口软糖</p>
//<p style=""text-align: center;"">
//	12、似是故人来</p>
//<p style=""text-align: center;"">
//	13、缓慢的击中</p>
//<p style=""text-align: center;"">
//	14、江上晚风吟</p>
//<p style=""text-align: center;"">
//	15、只为伊人醉</p>
//<p style=""text-align: center;"">
//	16、青丝到白发</p>
//<p style=""text-align: center;"">
//	17、再次见初心</p>
//<p style=""text-align: center;"">
//	18、余生归故里</p>
//<p style=""text-align: center;"">
//	19、一见不钟情</p>
//<p style=""text-align: center;"">
//	20、酒酿樱桃子</p>
//<p style=""text-align: center;"">
//	21、一个人的梦</p>
//<p style=""text-align: center;"">
//	22、天依旧很蓝</p>
//<p style=""text-align: center;"">
//	23、聆听挽歌空</p>
//<p style=""text-align: center;"">
//	24、被岁月堵住</p>
//<p style=""text-align: center;"">
//	25、执伞青衣袖</p>
//<p style=""text-align: center;"">
//	26、酌墨花未央</p>
//<p style=""text-align: center;"">
//	27、安稳度余生</p>
//<p style=""text-align: center;"">
//	28、途经那小镇</p>
//<p style=""text-align: center;"">
//	29、青鸾萦篱落</p>
//<p style=""text-align: center;"">
//	30、未来不会来</p>
//<p style=""text-align: center;"">
//	31、相忘于江湖</p>
//<p style=""text-align: center;"">
//	32、夜深千帐灯</p>
//<p style=""text-align: center;"">
//	33、人群里消失</p>
//<p style=""text-align: center;"">
//	34、没那么简单</p>
//<p style=""text-align: center;"">
//	35、归期未有期</p>
//<p style=""text-align: center;"">
//	36、眉目做山河</p>
//<p style=""text-align: center;"">
//	37、白马啸西风</p>
//<p style=""text-align: center;"">
//	38、突然很想你</p>
//<p style=""text-align: center;"">
//	39、浅浅一道伤</p>
//<p style=""text-align: center;"">
//	40、不羁的青春</p>
//<p style=""text-align: center;"">
//	41、记忆拾荒者</p>
//<p style=""text-align: center;"">
//	42、寒楼挽歌忘</p>
//<p style=""text-align: center;"">
//	43、薄荷味日记</p>
//<p style=""text-align: center;"">
//	44、今生共相伴</p>
//<p style=""text-align: center;"">
//	45、学着去遗忘</p>
//<p style=""text-align: center;"">
//	46、浪漫归属者</p>
//<p style=""text-align: center;"">
//	47、我独一无二</p>
//<p style=""text-align: center;"">
//	48、孤影里有谁</p>
//<p style=""text-align: center;"">
//	49、回首恨伊人</p>
//<p style=""text-align: center;"">
//	50、花季花未开</p>
//<p style=""text-align: center;"">
//	51、你知我几分</p>
//<p style=""text-align: center;"">
//	52、错误的时光</p>
//<p style=""text-align: center;"">
//	53、怎么说出口</p>
//<p style=""text-align: center;"">
//	54、红尘为君舞</p>
//<p style=""text-align: center;"">
//	55、繁苒挽歌藏</p>
//<p style=""text-align: center;"">
//	56、谁饮一壶酒</p>
//<p style=""text-align: center;"">
//	57、雾散梦千秋</p>
//<p style=""text-align: center;"">
//	58、趁时光还在</p>
//<p style=""text-align: center;"">
//	59、甜的很正经</p>
//<p style=""text-align: center;"">
//	60、一朵无名花</p>
//<p style=""text-align: center;"">
//	61、有故事的人</p>
//<p style=""text-align: center;"">
//	62、白了少年头</p>
//<p style=""text-align: center;"">
//	63、陌上人如玉</p>
//<p style=""text-align: center;"">
//	64、花落亦不离</p>
//<p style=""text-align: center;"">
//	65、墨衣挽歌长</p>
//<p style=""text-align: center;"">
//	66、其实你不懂</p>
//<p style=""text-align: center;"">
//	67、捎信与候鸟</p>
//<p style=""text-align: center;"">
//	68、木槿花向晚</p>
//<p style=""text-align: center;"">
//	69、在我怀里睡</p>
//<p style=""text-align: center;"">
//	70、倒霉的天鹅</p>
//<p style=""text-align: center;"">
//	71、相忘尘世间</p>
//<p style=""text-align: center;"">
//	72、知南温茶暖</p>
//<p style=""text-align: center;"">
//	73、细雨斜阳愁</p>
//<p style=""text-align: center;"">
//	74、醉望挽歌路</p>
//<p style=""text-align: center;"">
//	75、岁月不待人</p>
//<p style=""text-align: center;"">
//	76、洛洛微光暖</p>
//<p style=""text-align: center;"">
//	77、眼泪是多余</p>
//<p style=""text-align: center;"">
//	78、走散在风中</p>
//<p style=""text-align: center;"">
//	79、云深不知处</p>
//<p style=""text-align: center;"">
//	80、若年华倒带</p>
//<p style=""text-align: center;"">
//	81、花落一瞬间</p>
//<p style=""text-align: center;"">
//	82、旁人怎会懂</p>
//<p style=""text-align: center;"">
//	83、故事总有你</p>
//<p style=""text-align: center;"">
//	84、花开的正好</p>
//<p style=""text-align: center;"">
//	85、回眸醉倾城</p>
//<p style=""text-align: center;"">
//	86、浅浅初荷岚</p>
//<p style=""text-align: center;"">
//	87、长发绾青丝</p>
//<p style=""text-align: center;"">
//	88、岁月你别催</p>
//<p style=""text-align: center;"">
//	89、风月不等闲</p>
//<p style=""text-align: center;"">
//	90、逆风里并肩</p>
//<p style=""text-align: center;"">
//	91、薄薄白衣叹</p>
//<p style=""text-align: center;"">
//	92、时光里的夏</p>
//<p style=""text-align: center;"">
//	93、吹落南风中</p>
//<p style=""text-align: center;"">
//	94、爱你我怕了</p>
//<p style=""text-align: center;"">
//	95、碧落舞九天</p>
//<p style=""text-align: center;"">
//	96、总得有挣扎</p>
//<p style=""text-align: center;"">
//	97、未曾走近你</p>
//<p style=""text-align: center;"">
//	98、春风正得意</p>
//<p style=""text-align: center;"">
//	99、挽歌叹无泪</p>
//<p style=""text-align: center;"">
//	100、表面不在乎</p>
//<p style=""text-align: center;"">
//	101、仅有的温柔</p>
//<p style=""text-align: center;"">
//	102、试探的温柔</p>
//<p style=""text-align: center;"">
//	103、微凉的夏日</p>
//<p style=""text-align: center;"">
//	104、看不到未来</p>
//<p style=""text-align: center;"">
//	105、一脸美人痣</p>
//<p style=""text-align: center;"">
//	106、一袖南烟顾</p>
//<p style=""text-align: center;"">
//	107、第二眼沦陷</p>
//<p style=""text-align: center;"">
//	108、蜜桃奶油酱</p>
//<p style=""text-align: center;"">
//	109、谁将烟焚散</p>
//<p style=""text-align: center;"">
//	110、素手披薄衣</p>
//<p style=""text-align: center;"">
//	111、难过自己藏</p>
//<p style=""text-align: center;"">
//	112、还可以问候</p>
//<p style=""text-align: center;"">
//	113、何以顾人心</p>
//<p style=""text-align: center;"">
//	114、假如你还在</p>
//<p style=""text-align: center;"">
//	115、袭琼花欲落</p>
//<p style=""text-align: center;"">
//	116、吻你眉间风</p>
//<p style=""text-align: center;"">
//	117、醉后的温柔</p>
//<p style=""text-align: center;"">
//	118、浮生寄旧梦</p>
//<p style=""text-align: center;"">
//	119、苍白的诺言</p>
//<p style=""text-align: center;"">
//	120、青染苍白颜</p>
//<p style=""text-align: center;"">
//	121、戏落人幕终</p>
//<p style=""text-align: center;"">
//	122、星星打烊了</p>
//<p style=""text-align: center;"">
//	123、不尽相思灰</p>
//<p style=""text-align: center;"">
//	124、思念变成海</p>
//<p style=""text-align: center;"">
//	125、空城守旧梦</p>
//<p style=""text-align: center;"">
//	126、夏末的晨曦</p>
//<p style=""text-align: center;"">
//	127、与你太认真</p>
//<p style=""text-align: center;"">
//	128、南城旧少年</p>
//<p style=""text-align: center;"">
//	129、键盘江山刀</p>
//<p style=""text-align: center;"">
//	130、久伴我心安</p>
//<p style=""text-align: center;"">
//	131、单方的守候</p>
//<p style=""text-align: center;"">
//	132、赴一场爱意</p>
//<p style=""text-align: center;"">
//	133、心声谁要听</p>
//<p style=""text-align: center;"">
//	134、精灵入我梦</p>
//<p style=""text-align: center;"">
//	135、花莞杳萝烟</p>
//<p style=""text-align: center;"">
//	136、窗外的过客</p>
//<p style=""text-align: center;"">
//	137、做你怀中猫</p>
//<p style=""text-align: center;"">
//	138、走在冷风中</p>
//<p style=""text-align: center;"">
//	139、陌上桃花开</p>
//<p style=""text-align: center;"">
//	140、无聊也不聊</p>
//<p style=""text-align: center;"">
//	141、与孤独为友</p>
//<p style=""text-align: center;"">
//	142、借一盏月色</p>
//<p style=""text-align: center;"">
//	143、你的小仙女</p>
//<p style=""text-align: center;"">
//	144、风落尘归去</p>
//<p style=""text-align: center;"">
//	145、萌到你眼炸</p>
//<p style=""text-align: center;"">
//	146、初逝的格调</p>
//<p style=""text-align: center;"">
//	147、动情却是空</p>
//<p style=""text-align: center;"">
//	148、残花零落叶</p>
//<p style=""text-align: center;"">
//	149、熬几个春秋</p>
//<p style=""text-align: center;"">
//	150、赴月观长安</p>
//<p style=""text-align: center;"">
//	151、听说你来过</p>
//<p style=""text-align: center;"">
//	152、玉壶酌浅溪</p>
//<p style=""text-align: center;"">
//	153、永夜月同孤</p>
//<p style=""text-align: center;"">
//	154、话别说太满</p>
//<p style=""text-align: center;"">
//	155、繁琐的年哗</p>
//<p style=""text-align: center;"">
//	156、风终将吹来</p>
//<p style=""text-align: center;"">
//	157、岂言不相思</p>
//<p style=""text-align: center;"">
//	158、南风吹北巷</p>
//<p style=""text-align: center;"">
//	159、所念皆星河</p>
//<p style=""text-align: center;"">
//	160、潇潇沐雨寒</p>
//<p style=""text-align: center;"">
//	161、淡雾染层林</p>
//<p style=""text-align: center;"">
//	162、细雨挽轻裳</p>
//<p style=""text-align: center;"">
//	163、烟花透骨寒</p>
//<p style=""text-align: center;"">
//	164、月色映归客</p>
//<p style=""text-align: center;"">
//	165、心事付瑶琴</p>
//<p style=""text-align: center;"">
//	166、泛白的记忆</p>
//<p style=""text-align: center;"">
//	167、镜子里的我</p>
//<p style=""text-align: center;"">
//	168、蜜心萝莉酱</p>
//<p style=""text-align: center;"">
//	169、心跳的感觉</p>
//<p style=""text-align: center;"">
//	170、海一样的人</p>
//<p style=""text-align: center;"">
//	171、水漾月微醺</p>
//<p style=""text-align: center;"">
//	172、末骤雨初歇</p>
//<p style=""text-align: center;"">
//	173、思君不见君</p>
//<p style=""text-align: center;"">
//	174、吃尽情字苦</p>
//<p style=""text-align: center;"">
//	175、盏灯照挽歌</p>
//<p style=""text-align: center;"">
//	176、温一壶月光</p>
//<p style=""text-align: center;"">
//	177、最初的模样</p>
//<p style=""text-align: center;"">
//	178、对此欢终宴</p>
//<p style=""text-align: center;"">
//	179、满园山茶俏</p>
//<p style=""text-align: center;"">
//	180、秋影唯挽歌</p>
//<p style=""text-align: center;"">
//	181、清清茶烟挽</p>
//<p style=""text-align: center;"">
//	182、等一个晴天</p>
//<p style=""text-align: center;"">
//	183、温柔了岁月</p>
//<p style=""text-align: center;"">
//	184、斯人独憔悴</p>
//<p style=""text-align: center;"">
//	185、辣条小仙女</p>
//<p style=""text-align: center;"">
//	186、干净也爱笑</p>
//<p style=""text-align: center;"">
//	187、得不到回应</p>
//<p style=""text-align: center;"">
//	188、匆匆那些年</p>
//<p style=""text-align: center;"">
//	189、一直很安静</p>
//<p style=""text-align: center;"">
//	190、明月下西楼</p>
//<p style=""text-align: center;"">
//	191、一笑为红颜</p>
//<p style=""text-align: center;"">
//	192、只剩下空心</p>
//<p style=""text-align: center;"">
//	193、情若能自控</p>
//<p style=""text-align: center;"">
//	194、我们回不去</p>
//<p style=""text-align: center;"">
//	195、很酷不聊天</p>
//<p style=""text-align: center;"">
//	196、偶遇在街角</p>
//<p style=""text-align: center;"">
//	197、愿红颜未老</p>
//<p style=""text-align: center;"">
//	198、最初的相遇</p>
//<p style=""text-align: center;"">
//	199、断魂红尘梦</p>
//<p style=""text-align: center;"">
//	200、挽歌萦绕耳</p>";

//			List<string> strings = new List<string>();
//			var sss = ss.Split("</p>");
//			for (int i = 0; i < sss.Length - 1; i++)
//			{
//				var grg = sss[i].Split("、");
//				var zz = grg[1];
//				strings.Add(zz);

//			}
//            await userDbContext.RandomUserNames.AddRangeAsync(strings.Select(s => new RandomUserName() { UserName = s}));


//            List<RandomUserAvatar> randomUserAvatars = new List<RandomUserAvatar>();
//            for (int i = 0; i < 21; i++)
//            {
//                randomUserAvatars.Add(new RandomUserAvatar() { AvatarUrl = $"Identity/Avatars/{i}.jpg" });
//            }
//            await userDbContext.RandomUserAvatars.AddRangeAsync(randomUserAvatars);
//        }
   
	
	}
}
