using SearchService.Domain.Entitis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Domain.IRepository
{
    public interface IRelevantSearchRepository
    {
        Task<bool> InsertAsync(SearchRecord searchKeyword);
        Task<(IEnumerable<SearchRecord> data, long total)> SearchAsync(string keyword);
        Task<SearchRecord?> SearchByRelevanSearchAsync(SearchRecord searchKeyword);
    }
}
