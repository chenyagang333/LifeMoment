
using Chen.EventBus;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;

namespace SearchService.WebAPI.EventHandlers
{
    /// <summary>
    /// 新增用户
    /// </summary>
    [EventName("UserDataInsert")]
    public class ListeningUserDataInsertEventHandler : JsonIntegrationEventHandler<LifeBusUser>
    {
        private readonly ISearchUserRepository repository;

        public ListeningUserDataInsertEventHandler(ISearchUserRepository repository)
        {
            this.repository = repository;
        }
        public override Task HandleJson(string eventName, LifeBusUser? eventData)
        {
            // 转换为string类型，这样可以通过IK分词器模糊匹配
            eventData.BuildUserAccountString();
            repository.InsertAsync(eventData);
            return Task.CompletedTask;
        }
    }

}
