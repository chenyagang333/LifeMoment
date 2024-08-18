namespace UserChatService.Domain.IService
{
    public interface IUserHubService
    {
        Task SendDataByUserIdAsync(IEnumerable<long> userIds, string method, object data);
        Task SendDataByUserIdAsync(long userId, string method, object data);
    }
}