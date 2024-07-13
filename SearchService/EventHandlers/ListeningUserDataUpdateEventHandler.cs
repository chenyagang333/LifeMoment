using Chen.EventBus;
using Elastic.Clients.Elasticsearch;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;

namespace SearchService.EventHandlers
{
    [EventName("UserDataUpdata_Description_FansCount_AttentionCount")]
    [EventName("UserDataUpdata_UserName_UserAvatar")]
    [EventName("UserDataUpdate_GetLikeCount")]
    public class ListeningUserDataUpdateEventHandler : JsonIntegrationEventHandler<UserDataUpdateEvent>
    {
        private readonly ISearchUserRepository repository;
        private readonly ElasticsearchClient elasticsearchClient;

        public ListeningUserDataUpdateEventHandler(ISearchUserRepository repository, ElasticsearchClient elasticsearchClient)
        {
            this.repository = repository;
            this.elasticsearchClient = elasticsearchClient;
        }


        public override async Task HandleJson(string eventName, UserDataUpdateEvent? eventData)
        {
            if (eventData != null)
            {
                object obj = GetUpdateField(eventData.Field, eventData.Value);
                if (obj != null)
                {
                    await repository.UpdateAsync(eventData.UserId, obj); // 
                    string[] strs1 = ["UserName", "UserAvatar"];
                    if (strs1.Contains(eventData.Field))
                    {
                        var mark = await MyHandler(obj, eventData);
                    }
                }
            }

        }

        private async Task<bool> MyHandler(object obj, UserDataUpdateEvent eventData)
        {
            string source;
            if (eventData.Field == "UserName")
            {
                source = "ctx._source.userName = params.value;";
            }
            else
            {
                source = "ctx._source.userAvatarURL = params.value";
                await Task.Delay(3000);
            }
            InlineScript inlineScript = new(source)
            {
                Params = new Dictionary<string, object>
                            {
                                { "value", eventData.Value }
                            },
            };
            Script script = new(inlineScript);
            var res = await elasticsearchClient.UpdateByQueryAsync<Strength>(u => u
                .Indices("strengths")
                .Query(q => q.Term(t => t.Field(f => f.UserId).Value(eventData.UserId).Suffix("keyword")))
                .Script(script)
                );
            return res.IsValidResponse;
        }

        private object? GetUpdateField(string field, dynamic data)
        {
            if (field == "GetLikeCount")
                return new { GetLikeCount = data };
            if (field == "UserName")
                return new { UserName = data };
            if (field == "UserAvatar")
                return new { UserAvatar = data };
            if (field == "Description")
                return new { Description = data };
            if (field == "FansCount")
                return new { FansCount = data };
            if (field == "AttentionCount")
                return new { AttentionCount = data };
            return default;
        }
    }
    public record UserDataUpdateEvent(long UserId, string Field, dynamic Value);

}
