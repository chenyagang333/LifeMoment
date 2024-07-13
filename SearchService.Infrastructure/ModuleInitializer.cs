using Chen.Commons;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;
using SearchService.Domain.IServices;
using SearchService.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped(sp =>
            {
                var option = sp.GetRequiredService<IOptions<ElasticSearchOptions>>();
                // 拿到服务器地址
                var url = option.Value.Url;
                // 配置默认索引名
                ElasticsearchClientSettings elasticsearchClientSettings =
                new ElasticsearchClientSettings(url)
                .DefaultIndex("strengths").DefaultMappingFor<Strength>(m => m
                .IndexName("strengths").IdProperty(p => p.Id)
                );
                return new ElasticsearchClient(elasticsearchClientSettings);
            });
            services.AddScoped<ISearchRepository,SearchRepository>();
            services.AddScoped<ISearchUserRepository, SearchUserRepository>();
            services.AddScoped<IRelevantSearchRepository, RelevantSearchRepository>();
            services.AddScoped<ISearchService, Services.SearchService>();
        }
    }
}
