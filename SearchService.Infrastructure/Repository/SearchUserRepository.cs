using Chen.DomainCommons.ConfigOptions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Infrastructure.Repository
{
    public class SearchUserRepository : ISearchUserRepository
    {
        private readonly ElasticsearchClient elasticsearchClient;
        private readonly IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions;

        public SearchUserRepository(ElasticsearchClient elasticsearchClient,
            IOptionsSnapshot<FileServiceCommonOptions> fileServerOptions
            )
        {
            this.elasticsearchClient = elasticsearchClient;
            this.fileServerOptions = fileServerOptions;
        }
        public Task DeleteAsync(long id)
        {
            return elasticsearchClient.DeleteAsync("users", id);
        }

        public async Task InsertAsync(LifeBusUser user)
        {
            // 新增数据
            var response = await elasticsearchClient.IndexAsync(user, "users", user.Id);
        }

        public async Task UpdateAsync(long id, object obj)
        {
            var response = await elasticsearchClient.UpdateAsync<LifeBusUser, object>("users", id, u => u
               .Doc(obj));
        }


        public async Task<(IEnumerable<LifeBusUser>, long totalCount)>
            SearchUsers(string sort, string keyword, int pageIndex, int pageSize)
        {
            int from = pageSize * (pageIndex - 1);
            // 查询关键字
            Action<QueryDescriptor<LifeBusUser>> actionQuery = q => q.Bool(b =>
            {
                b.Should(
                    should => should.Match(m => m.Field(f => f.UserName).Query(keyword)),
                    should => should.Match(m => m.Field(f => f.UserAccountString).Query(keyword)),
                    should => should.Match(m => m.Field(f => f.Email).Query(keyword)),
                    should => should.Match(m => m.Field(f => f.PhoneNumber).Query(keyword)),
                    should => should.Match(m => m.Field(f => f.Description).Query(keyword))
                );

            });

            // 高亮关键字
            //Action<HighlightDescriptor<Strength>> actionHighlight =
            //    h => h.Fields(fs => fs);
            // 搜索
            var res = await elasticsearchClient.SearchAsync<LifeBusUser>(s => s
            .Index("users")
            .From(from)
            .Size(pageSize)
            .Query(actionQuery)
            //.Sort(GetSortOptionsDescriptor(sort))
            //.Highlight(actionHighlight)
            );
            if (!res.IsValidResponse)
            {
                throw new ApplicationException(res.DebugInformation);
            }
            var docList = res.Documents?.ToList() ?? [];
            if (docList.Count > 0)
            {
                docList.ForEach(x => x.SpliceUserAvatarURL(fileServerOptions.Value.FileBaseUrl));
            }
            return (docList, res.Total);
        }

    }
}
