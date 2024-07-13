using AutoMapper;
using Chen.Commons.FileUtils;
using IdentityService.Domain.DTO;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Notifications;
using System.Text.Json;

namespace IdentityService.WebAPI
{

    public class CustomProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<User, AddUserEvent>();

        }
  
    }

}
