using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.Commons.ApiResult.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SearchService.Domain.Entitis;
using SearchService.Domain.IRepository;
using SearchService.Domain.IServices;
using SearchService.Infrastructure;
using SearchService.Infrastructure.Services;
using SearchService.Request;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SearchService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository repository;
        private readonly ISearchUserRepository searchUserRepository;
        private readonly IRelevantSearchRepository relevantSearchRepository;
        private readonly ISearchService searchService;

        public SearchController(ISearchRepository repository,
            ISearchUserRepository searchUserRepository,
            IRelevantSearchRepository relevantSearchRepository,
            ISearchService searchService)
        {
            this.repository = repository;
            this.searchUserRepository = searchUserRepository;
            this.relevantSearchRepository = relevantSearchRepository;
            this.searchService = searchService;
        }


        /// <summary>
        /// 获取文章信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiListResult<IEnumerable<Strength>>>> SearchStrengths([FromQuery] SearchStrengthsRequest req)
        {
            searchService.SearchByRelevanSearchAsync(req.keyword);
            var (strengths, total) = await repository.SearchStrengths(req.sort, req.keyword, req.pageIndex, req.pageSize);
            return ApiListResult<IEnumerable<Strength>>.Succeeded(strengths,total);

        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiListResult<IEnumerable<LifeBusUser>>>> SearchUsers([FromQuery] SearchUsersRequest req)
        {
            searchService.SearchByRelevanSearchAsync(req.keyword);
            var (users, total) = await searchUserRepository.SearchUsers(req.sort, req.keyword, req.pageIndex, req.pageSize);
            return ApiListResult<IEnumerable<LifeBusUser>>.Succeeded(users, total);

        }

        /// <summary>
        /// 获取相关搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
         [HttpGet]
        public async Task<ActionResult<ApiListResult<IEnumerable<SearchRecord>>>> GetRelevantSearch(string keyword)
        {
            var (data,total) = await relevantSearchRepository.SearchAsync(keyword);
            return ApiListResult<IEnumerable<SearchRecord>>.Succeeded(data, total);
        }



    }
}
