using Chen.EventBus;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;

namespace SearchService.WebAPI.EventHandlers
{
    /// <summary>
    /// 插入文章
    /// </summary>
    [EventName("ListeningStrength.Insert")]
    public class ListeningStrengthInsertEventHandler : JsonIntegrationEventHandler<Strength>
    {
        private readonly ISearchRepository repository;
        public ListeningStrengthInsertEventHandler(ISearchRepository repository)
        {
            this.repository = repository;
        }


        public override Task HandleJson(string eventName, Strength? eventData)
        {
            if (eventData == null) { return Task.CompletedTask; }
            return repository.UpsertAsync(eventData);
        }
    }
}
