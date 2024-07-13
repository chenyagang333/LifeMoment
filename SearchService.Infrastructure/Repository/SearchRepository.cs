using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.Fluent;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SearchService.Infrastructure.Repository
{
    public class SearchRepository : ISearchRepository
    {
        private readonly ElasticsearchClient elasticsearchClient;

        public SearchRepository(ElasticsearchClient elasticsearchClient)
        {
            this.elasticsearchClient = elasticsearchClient;
        }

        // 删除数据
        public Task DeleteAsync(long strengthId)
        {
            return elasticsearchClient.DeleteAsync("strengths", strengthId);
        }

        // 新增数据
        public async Task InsertAsync(Strength strength)
        {
            var response = await elasticsearchClient.IndexAsync(strength, "strengths", strength.Id);

            if (response.IsValidResponse)
            {
                Console.WriteLine($"Index document with ID {response.Id} succeeded.");
            }
        }

        public async Task<(IEnumerable<Strength>, long totalCount)>
            SearchStrengths(string sort, string keyword, int pageIndex, int pageSize)
        {
            int from = pageSize * (pageIndex - 1);
            // 查询关键字
            Action<QueryDescriptor<Strength>> actionQuery = q => q.Bool(b =>
            {
                b.Should(
                    should => should.Match(m => m.Field(f => f.UserName).Query(keyword)),
                    should => should.Match(m => m.Field(f => f.Content).Query(keyword))
                );

            });

            // 高亮关键字
            //Action<HighlightDescriptor<Strength>> actionHighlight =
            //    h => h.Fields(fs => fs);
            // 搜索
            var res = await elasticsearchClient.SearchAsync<Strength>(s => s
            .Index("strengths")
            .From(from)
            .Size(pageSize)
            .Query(actionQuery)
            .Sort(GetSortOptionsDescriptor(sort))
            //.Highlight(actionHighlight)
            );
            if (!res.IsValidResponse)
            {
                throw new ApplicationException(res.DebugInformation);
            }
            var docList = res.Documents?.ToList();

            //var res = await elasticsearchClient.SearchAsync<Strength>(s => s
            //.Index("strengths")
            //.From(from)
            //.Size(pageSize));
            //if (!res.IsValidResponse)
            //{
            //    throw new ApplicationException(res.DebugInformation);
            //}
            //var docList = res.Documents?.ToList();
            return (docList, res.Total);
        }


        // 更新数据
        public async Task UpdateAsync(Strength strength)
        {
            var response = await elasticsearchClient.UpdateAsync<Strength, Strength>("strengths", strength.Id, u => u
            .Doc(strength));
            if (response.IsValidResponse)
            {
                Console.WriteLine("Update document succeeded.");
            }
        }

        // 更新数据
        public async Task UpdateAsync(long id, object obj)
        {
            var response = await elasticsearchClient.UpdateAsync<Strength, object>("strengths", id, u => u
                      .Doc(obj));
        }

        //public async Task UpdateAsync123(long id,dynamic filedData,)
        //{
        //    var response = await elasticsearchClient.UpdateAsync<Strength, object>("strengths", strength.Id, u => u
        //    .Doc(new {}));
        //    if (response.IsValidResponse)
        //    {
        //        Console.WriteLine("Update document succeeded.");
        //    }
        //}

        // 新增或覆盖（更新）
        public async Task UpsertAsync(Strength strength)
        {
            // Upsert:Update or Insert
            var res = await elasticsearchClient.IndexAsync(strength, index => index.Index("strengths").Id(strength.Id));
            if (!res.IsValidResponse)
            {
                throw new ApplicationException(res.DebugInformation);
            }
        }

        // 获取排序的 描述信息
        private Action<SortOptionsDescriptor<Strength>>[] GetSortOptionsDescriptor(string sort = "default")
        {
            List<Action<SortOptionsDescriptor<Strength>>> actions = new();

            if (sort == "default") // 综合排序
            {
                actions = new()
                {
                    s => s.Field(f => f.LikeCount, s => s.Order(SortOrder.Desc)),
                    s => s.Field(f => f.CommentCount, s => s.Order(SortOrder.Desc)),
                    s => s.Field(f => f.StarCount, s => s.Order(SortOrder.Desc)),
                    s => s.Field(f => f.ViewCount, s => s.Order(SortOrder.Desc)),
                    s => s.Field(f => f.CreateTime, s => s.Order(SortOrder.Desc)),
                };
            }
            else if (sort == "MostLikes") // 最多喜欢
            {
                actions.Add(s => s.Field(f => f.LikeCount, s => s.Order(SortOrder.Desc)));
            }
            else if (sort == "MostComments") // 最多评论
            {
                actions.Add(s => s.Field(f => f.CommentCount, s => s.Order(SortOrder.Desc)));
            }
            else if (sort == "NewPublish") // 最新发布
            {
                actions.Add(s => s.Field(f => f.CreateTime, s => s.Order(SortOrder.Desc)));
            }
            else if (sort == "MostStars") // 最多收藏
            {
                actions.Add(s => s.Field(f => f.StarCount, s => s.Order(SortOrder.Desc)));
            }
            else if (sort == "MostViews") // 最多浏览
            {
                actions.Add(s => s.Field(f => f.ViewCount, s => s.Order(SortOrder.Desc)));
            }

            return actions.ToArray(); ;
        }


    }
}
