using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchService.Domain.Entitis;

namespace SearchService.Domain.IServices
{
    public interface ISearchService
    {
        Task SearchByRelevanSearchAsync(string keyword);
    }
}
