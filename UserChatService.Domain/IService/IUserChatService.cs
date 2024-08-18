namespace UserChatService.Domain.IService
{
    public interface IUserChatService
    {
        Task<IEnumerable<dynamic>> GetDialogAndGroupsByUserIdAsync(long userId);
    }
}