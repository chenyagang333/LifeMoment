using Chen.EventBus;
using MediatR;
using SearchService.Domain.IRepository;

namespace SearchService.WebAPI.EventHandlers
{
    /// <summary>
    /// 删除文章
    /// </summary>
    [EventName("ListeningStrength.Deleted")]
    [EventName("ListeningStrength.Hidden")] // 隐藏也看做删除
    public class ListeningStrengthDeletedEventHandler:DynamicIntegrationEventHandler
    {
        private readonly ISearchRepository repository;

        public ListeningStrengthDeletedEventHandler(ISearchRepository repository)
        {
            this.repository = repository;
        }

        public override Task HandleDynamic(string eventName, dynamic StrengthId)
        {
            long Id = long.Parse(StrengthId);
            return repository.DeleteAsync(Id);
        }
    }


}
