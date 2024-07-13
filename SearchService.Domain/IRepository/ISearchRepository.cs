using SearchService.Domain.Entitis;

namespace SearchService.Domain.IRepository
{
    public interface ISearchRepository
    {
        Task UpsertAsync(Strength strength);
        Task InsertAsync(Strength strength);
        Task UpdateAsync(Strength strength);
        Task UpdateAsync(long id, object obj);
        Task DeleteAsync(long strengthId);
        Task<(IEnumerable<Strength>, long totalCount)> SearchStrengths(string sort, string keyword, int pageIndex, int pageSize);
    }
}
