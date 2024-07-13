using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SearchService.Infrastructure.Repository
{
    internal class RelevantSearchRepository : IRelevantSearchRepository
    {
        private readonly ElasticsearchClient elasticsearchClient;

        public RelevantSearchRepository(ElasticsearchClient elasticsearchClient)
        {
            this.elasticsearchClient = elasticsearchClient;
        }
        public async Task<bool> InsertAsync(SearchRecord relevanSearch)
        {
            // 新增数据
            var res = await elasticsearchClient.IndexAsync(relevanSearch, index => index.Index("relevan-searchs"));
            if (res.IsValidResponse)
            {
                return true;
            }
            return false;
        }

        public async Task<(IEnumerable<SearchRecord> data, long total)> SearchAsync(string keyword)
        {
            // 查询关键字
            Action<QueryDescriptor<SearchRecord>> actionQuery = q => q.Match(m => m.Field(x => x.RelevanSearchWord).Query(keyword));

            // 高亮关键字
            Action<HighlightDescriptor<SearchRecord>> actionHighlightDescriptor = h => h
                .BoundaryScannerLocale("zh-CN")
                .PreTags(["<em>"])
                .PostTags(["<em/>"])
                .HighlightQuery(hq => hq.Bool(bl => bl.Should(s => s.Match(m => m.Field(f => f.RelevanSearchWord).Query(keyword)))));

            // 搜索
            var res = await elasticsearchClient.SearchAsync<SearchRecord>(s => s
            .Index("relevan-searchs")
            .Query(actionQuery)
            //.Highlight(actionHighlightDescriptor)
            );
            if (!res.IsValidResponse)
            {
                throw new ApplicationException(res.DebugInformation);
            }
            var docList = res.Documents?.ToList();
            return (docList, res.Total);
        }

        public async Task<SearchRecord?> SearchByRelevanSearchAsync(SearchRecord searchKeyword)
        {
            //TermQuery termQuery = new("relevanSearchWord") { Value = searchKeyword.RelevanSearchWord };
            var response = await elasticsearchClient.SearchAsync<SearchRecord>(s => s
                .Index("relevan-searchs")
                .Query(q => q
                .Term(t => t.Field(f => f.RelevanSearchWord.Suffix("keyword")).Value(searchKeyword.RelevanSearchWord))
                )
            );
            if (response.IsValidResponse)
            {
                var data = response.Documents.FirstOrDefault();
                return data;
            }
            return default;
        }
    }
}
