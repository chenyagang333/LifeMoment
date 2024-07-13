using Azure.Core;
using Chen.Commons.ApiResult;
using FileService.Domain;
using FileService.Domain.Entities;
using FileService.Domain.IService;
using FileService.Infrastructure;
using FileService.WebAPI.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FileService.WebAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploaderController : ControllerBase
    {
        private readonly IFSDomainService fSDomainService;
        private readonly FSDbContext ctx;
        private readonly IFSRespository fSRespository;

        public UploaderController(IFSDomainService fSDomainService, FSDbContext ctx, IFSRespository fSRespository)
        {
            this.fSDomainService = fSDomainService;
            this.ctx = ctx;
            this.fSRespository = fSRespository;
        }
        // 检查是否已存在该文件
        [HttpGet]
        public async Task<ActionResult<ApiResult>> FileExists(long fileSize, string sha256Hash)
        {
            var item = await fSRespository.FindFileAsync(fileSize, sha256Hash);
            if (item == null) 
            {
                return ApiResult.Error;
            }
            return ApiResult.Succeeded(item);
        }


        /// <summary>
        /// 支持图片视频上传，如果存在视频，则视频文件的下一个文件为视频文件的封面
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<ApiResult>> MultipleUpload([FromForm] MultipleUploadRequests requests, CancellationToken cancellationToken = default)
        {
            var ds = Request.Form;
            var data = await fSDomainService.UploadAsync(requests.ServiceType, requests.Files, cancellationToken);
            await ctx.SaveChangesAsync();
            return ApiResult.Succeeded(data);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<ApiResult>> UploadImage([FromForm] ImageUploadRequest request, CancellationToken cancellationToken = default)
        {

            var data = await fSDomainService.ImageUploadAsync(request.ServiceType, request.File, cancellationToken);
            await ctx.SaveChangesAsync();
            return ApiResult.Succeeded(data);
        }

        /// <summary>
        /// 上传视频文件（包含视频封面图片文件）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(60_000_000)]
        public async Task<ActionResult<ApiResult>> UploadVideo([FromForm] VideoUploadRequest request, CancellationToken cancellationToken = default)
        {

            var data = await fSDomainService.VideoUploadAsync(request.ServiceType, request.Files, cancellationToken);
            await ctx.SaveChangesAsync();
            return ApiResult.Succeeded(data);
        }
    }
}
