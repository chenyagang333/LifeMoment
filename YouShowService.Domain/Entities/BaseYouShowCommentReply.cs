using Chen.Commons.FileUtils;
using Chen.DomainCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Notifications;

namespace YouShowService.Domain.Entities;
public record BaseYouShowCommentReply : AggregateRootEntity
{
    public long UserId { get;  set; }
    public string? UserName { get;  set; }
    public string? UserAvatarURL { get;  set; }
    public string? PublishAddress { get;  set; }
    public string? Content { get;  set; }
    public int LikeCount { get;  set; }




    public BaseYouShowCommentReply UpdateUserName(string userName)
    {
        UserName = userName;
        return this;
    }
    public BaseYouShowCommentReply UpdateUserAvatarURL(string userAvatarURL)
    {
        UserAvatarURL = userAvatarURL;
        return this;
    }
    public BaseYouShowCommentReply UpdateContent(string content)
    {
        Content = content;
        return this;
    }
    public BaseYouShowCommentReply UpdatePublishAddress(string? publishAddress)
    {
        if (!string.IsNullOrEmpty(publishAddress))
        {
            PublishAddress = publishAddress;
        }
        return this;
    }
    public BaseYouShowCommentReply UpdateUserId(long userId)
    {
        UserId = userId;
        return this;
    }

    public BaseYouShowCommentReply SpliceUserAvatarURL(string baseUrl)
    {
        if (!string.IsNullOrEmpty(UserAvatarURL))
        {
            UserAvatarURL = baseUrl + UserAvatarURL;
        }
        return this;
    }





}

