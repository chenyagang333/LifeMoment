using AutoMapper;
using IdentityService.Domain.DTO.UserChat;
using IdentityService.Domain.Entities.UserChat;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.DTO.ACustomProfile
{

    public class UserChatProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public UserChatProfile()
        {
            // 用户对话表
            CreateMap<UserDialogToUser, UserDialogToUserDTO>();
            // 用户群聊表
            CreateMap<UserGroupsToUser, UserGroupsToUserDTO>();
            // 用户群聊信息映射
            CreateMap<UserGroupsMessage, UserGroupsMessageDTO>();
        }

    }
}
