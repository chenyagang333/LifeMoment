using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;
using SearchService.Domain.IServices;
using SearchService.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SearchService.Infrastructure.Services
{
    public class SearchService : ISearchService
    {
        private readonly IRelevantSearchRepository relevantSearchRepository;

        public SearchService(IRelevantSearchRepository relevantSearchRepository)
        {
            this.relevantSearchRepository = relevantSearchRepository;
        }
        public async Task SearchByRelevanSearchAsync(string keyword)
        {
            var relevanSearch = new SearchRecord(keyword);
            var data = await relevantSearchRepository.SearchByRelevanSearchAsync(relevanSearch);
            if (data == null)
            {
                await relevantSearchRepository.InsertAsync(relevanSearch);
            }
        }
    }
}
