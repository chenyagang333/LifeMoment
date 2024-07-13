using Chen.EventBus;
using Elastic.Clients.Elasticsearch;
using MediatR;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;

namespace SearchService.EventHandlers
{
    /// <summary>
    /// 更新文章
    /// </summary>
    [EventName("ListeningStrength.Update")]
    public class ListeningStrengthUpdateEventHandler : JsonIntegrationEventHandler<YouShowUpdateEvent>
    {
        private readonly ISearchRepository repository;

        public ListeningStrengthUpdateEventHandler(ISearchRepository repository)
        {
            this.repository = repository;
        }

        public override async Task HandleJson(string eventName, YouShowUpdateEvent? data)
        {
            if (data != null)
            {
                var obj = GetUpdateField(data.type, data.UpdateData);
                if (obj != null)
                {
                    await repository.UpdateAsync(data.YouShowId,obj);
                }
            }
        }

        private object? GetUpdateField(YouShowUpdateEventType type, dynamic data)
        {
            if (type == YouShowUpdateEventType.UpdateLikeUsers)
                return new { LikeUsers = data };
            if (type == YouShowUpdateEventType.AddViewCount)
                return new { ViewCount = data };
            if (type == YouShowUpdateEventType.AddShareCount)
                return new { ShareCount = data };
            if (type == YouShowUpdateEventType.AddCommentCount)
                return new { CommentCount = data };
            if (type == YouShowUpdateEventType.AddLikeCount)
                return new { LikeCount = data };
            if (type == YouShowUpdateEventType.AddStarCount)
                return new { StarCount = data };
            return default;
        }
    }
    public record YouShowUpdateEvent(long YouShowId, dynamic UpdateData, YouShowUpdateEventType type);

    public enum YouShowUpdateEventType
    {
        UpdateLikeUsers,
        AddViewCount,
        AddShareCount,
        AddCommentCount,
        AddLikeCount,
        AddStarCount,
    }


}
