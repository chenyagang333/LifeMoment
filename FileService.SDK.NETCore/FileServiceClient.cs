using Chen.Commons;
using Chen.Commons.ApiResult;
using Chen.Commons.ApiResult.Generic;
using Chen.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace FileService.SDK.NETCore
{
    public class FileServiceClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Uri serverRoot;
        private readonly JWTOptions jWTOptions;
        private readonly ITokenService tokenService;

        public FileServiceClient(IHttpClientFactory httpClientFactory, Uri serverRoot, JWTOptions jWTOptions, ITokenService tokenService)
        {
            this.httpClientFactory = httpClientFactory;
            this.serverRoot = serverRoot;
            this.jWTOptions = jWTOptions;
            this.tokenService = tokenService;
        }

        public Task<ApiResult> FileExistsAsync(long fileSize, string sha256Hash, CancellationToken cancellationToken = default)
        {
            FormattableString url = $"api/Uploader/FileExists?fileSize={fileSize}&sha256Hash={sha256Hash}";
            string relativeUrl = FormattableStringHelper.BuildUrl(url);
            Uri requestUri = new Uri(serverRoot, relativeUrl);
            var httpClient = httpClientFactory.CreateClient();
            return httpClient.GetJsonAsync<ApiResult>(requestUri, cancellationToken)!;
        }

        public async Task<ApiResult<string>> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            using MultipartFormDataContent content = new();
            using StreamContent streamContent = new(file.OpenReadStream());
            content.Add(streamContent, "file", file.FileName);
            var httpClient = httpClientFactory.CreateClient();
            Uri requestUri = new(serverRoot, "/api/Uploader/Upload");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BuildToken());
            var respMsg = await httpClient.PostAsync(requestUri, content, cancellationToken);
            string respString = await respMsg.Content.ReadAsStringAsync(cancellationToken);
            if (!respMsg.IsSuccessStatusCode)
            {
                return ApiResult<string>.Failed($"上传失败，状态码：{respMsg.StatusCode}，响应报文：{respString}");
            }
            var respStr = respString.ToUpperOfJsonKey();
            var data = JsonSerializer.Deserialize<ApiResult<string>>(respStr)!;
            return data;
        }


        public async Task<ApiResult<string>> UploadWithRetryAsync(IFormFile file, int maxRetryAttempts = 1, CancellationToken cancellationToken = default)
        {
            int attempt = 0;
            bool whileIng = true;
            ApiResult<string> res = new();
            while (attempt < maxRetryAttempts && whileIng)
            {
                res = await UploadAsync(file, cancellationToken);
                if (res.Code == 200)
                {
                    whileIng = false;
                    return res;
                }
                await Task.Delay(200);
                attempt++;
            }
            return res;
        }

        public async Task<ApiResult> UploadFormFilesAsync(IFormFileCollection formFiles,
            int maxRetryAttempts = 1, CancellationToken cancellationToken = default)
        {
            List<string> urlList = new List<string>();
            foreach (var file in formFiles)
            {
                try
                {
                    var res = await UploadWithRetryAsync(file, maxRetryAttempts, cancellationToken);
                    if (res.Code == 200)
                    {
                        urlList.Add(res.Data);
                    }
                    else
                    {
                        return ApiResult.Failed(res.Message);
                    }
                }
                finally
                {
                }
            }
            return ApiResult.Succeeded(urlList);
        }


        string BuildToken()
        {
            // 因为 JWT 的信息只有服务端知道，因此可以非常简单的读到配置。
            Claim claim = new(ClaimTypes.Role, "admin");
            return tokenService.BuildToken([claim], jWTOptions);
        }
    }
}
