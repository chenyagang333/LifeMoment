using Chen.Commons;
using Chen.DomainCommons.Models;
using IdentityService.Domain.Notifications;
using IdentityService.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class User : IdentityUser<long>, IHasCreateTime, IHasDeleteTime, ISoftDelete, IDomainEvents
    {
        public User(string userName) : base(userName)
        {
            CreateTime = DateTime.Now;
        }

        public User Build()
        {

            return this;
        }

        /// <summary>
        /// 获取用户的创建时间
        /// </summary>
        public DateTime CreateTime { get; init; }

        /// <summary>
        /// 用户信息软删除时间
        /// </summary>
        public DateTime? DeletionTime { get; private set; }

        /// <summary>
        /// 获取或设置用户账号
        /// </summary>
        public int UserAccount { get; private set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserAvatar { get; private set; }

        /// <summary>
        /// 查看或设置该用户信息是否软删除
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// 关注数
        /// </summary>
        public int AttentionCount { get; set; }

        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FansCount { get; set; }

        /// <summary>
        /// 所有作品获赞总数量
        /// </summary>
        public long GetLikeCount { get; set; }

        /// <summary>
        /// 点赞作品的数量
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 收藏作品的数量
        /// </summary>
        public int StarCount { get; set; }

        /// <summary>
        /// 作品数量
        /// </summary>
        public int ContentCount { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        public string? Description { get; set; }



        /// <summary>
        /// 对用户信息进行软删除
        /// </summary>
        public void SoftDelete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.Now;
        }

        /// <summary>
        /// 判断密码是否为NullOrEmpty
        /// </summary>
        /// <returns>密码存在</returns>
        public bool HasPassword()
        {
            return !string.IsNullOrEmpty(PasswordHash);
        }

        public User UpdateUserName(string newUserName)
        {
            if (!string.IsNullOrEmpty(newUserName) && UserName != newUserName)
            {
                UserName = newUserName;
                AddDomainEventIfAbsent(new UserDataUpdataEvent(Id, "UserName", UserName));
            }
            return this;
        }


        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="newPassword"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public User ChangePassword(string newPassword)
        {
            if (newPassword.Length <= 6)
            {
                throw new ArgumentOutOfRangeException("密码长度必须大于6");
            }
            PasswordHash = HashHelper.ComputeMd5Hash(newPassword);
            return this;
        }

        /// <summary>
        /// 检查另一个密码与该用户的密码是否相同，是返回true，否则返回 false。
        /// </summary>
        /// <param name="anotherPassword">另一个密码</param>
        /// <returns>密码与该用户的密码是否相同，是返回true，否则返回 false。</returns>
        public bool CheckPassword(string anotherPassword)
        {
            if (PasswordHash == HashHelper.ComputeMd5Hash(anotherPassword))
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 更新用户邮箱
        /// </summary>
        /// <param name="email"></param>
        public User ChangeEmail(string email)
        {
            Email = email;
            return this;
        }

        /// <summary>
        /// 更新用户手机号
        /// </summary>
        /// <param name="phoneNumber"></param>
        public User ChangePhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            return this;
        }

        /// <summary>
        /// 更新用户账号
        /// </summary>
        /// <param name="newUserAccount"></param>
        public User ChangeUserAccount(int newUserAccount)
        {
            UserAccount = newUserAccount;
            return this;
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="newUserAvatar"></param>
        /// <returns></returns>
        public User ChangeUserAvatar(string newUserAvatar)
        {
            if (!string.IsNullOrEmpty(newUserAvatar) && UserAvatar != newUserAvatar)
            {
                UserAvatar = newUserAvatar;
                AddDomainEventIfAbsent(new UserDataUpdataEvent(Id, "UserAvatar", UserAvatar));
            }
            return this;
        }

        /// <summary>
        /// 添加关注数量
        /// </summary>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public User AddAttentionCount(int addCount)
        {
            AttentionCount += addCount;
            AddDomainEventIfAbsent(new UserDataUpdataEvent(Id, "AttentionCount", AttentionCount));
            return this;
        }

        /// <summary>
        /// 添加粉丝数量
        /// </summary>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public User AddFansCount(int addCount)
        {
            FansCount += addCount;
            AddDomainEventIfAbsent(new UserDataUpdataEvent(Id, "FansCount", FansCount));
            return this;
        }

        /// <summary>
        /// 添加获得总点赞数量
        /// </summary>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public User AddGetLikeCount(int addCount)
        {
            GetLikeCount += addCount;
            return this;
        }

        /// <summary>
        /// 添加点赞数量
        /// </summary>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public User AddLikeCount(int addCount)
        {
            LikeCount += addCount;
            return this;
        }

        public User UpdateLikeCount(int newCount)
        {
            LikeCount = newCount;
            return this;
        }

        /// <summary>
        /// 添加收藏数量
        /// </summary>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public User AddStarCount(int addCount)
        {
            StarCount += addCount;
            return this;
        }

        public User UpdateStarCount(int newCount)
        {
            StarCount = newCount;
            return this;
        }

        /// <summary>
        /// 添加作品数量
        /// </summary>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public User AddContentCount(int addCount)
        {
            ContentCount += addCount;
            return this;
        }

        public User UpdateContentCount(int newCount)
        {
            ContentCount = newCount;
            return this;
        }

        /// <summary>
        /// 更新个人简介
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public User UpdateDescription(string description)
        {
            if (description != Description)
            {
                Description = description;
                AddDomainEventIfAbsent(new UserDataUpdataEvent(Id, "Description", Description));
            }
            return this;
        }


        

        #region 领域事件模块

        [NotMapped]
        private List<INotification> domainEvents = new();


        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents.Add(eventItem);
        }

        public void AddDomainEventIfAbsent(INotification eventItem)
        {
            if (!domainEvents.Contains(eventItem))
            {
                domainEvents.Add(eventItem);
            }
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }

        public IEnumerable<INotification> GetDomainEvents()
        {
            return domainEvents;
        }

        #endregion
    }
}
