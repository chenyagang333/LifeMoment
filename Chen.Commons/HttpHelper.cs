using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Chen.Commons
{
    public static class HttpHelper
    {
        public static async Task SaveToFileAsync(this HttpResponseMessage respMsg, string file
            , CancellationToken cancellationToken = default)
        {
            if (!respMsg.IsSuccessStatusCode)
            {
                throw new ArgumentException($"StatusCode of HttpResponseMessage is {respMsg.StatusCode}", nameof(respMsg));
            }
            using FileStream fs = new FileStream(file, FileMode.Create);
            await respMsg.Content.CopyToAsync(fs, cancellationToken);
        }

        public static async Task<HttpStatusCode> DownloadFileAsync(this HttpClient httpClient, Uri url, string localFile
            , CancellationToken cancellationToken = default)
        {
            var resp = await httpClient.GetAsync(url, cancellationToken);
            if (resp.IsSuccessStatusCode)
            {
                await resp.SaveToFileAsync(localFile, cancellationToken);
                return resp.StatusCode;
            }
            else
            {
                return HttpStatusCode.OK;
            }
        }

        public static async Task<T?> GetJsonAsync<T>(this HttpClient httpClient, Uri url
            , CancellationToken cancellationToken = default)
        {
            string json = await httpClient.GetStringAsync(url, cancellationToken);
            return json.ParseJson<T>();
        }

        /// <summary>
        /// 获取IP所在的省份
        /// </summary>
        /// <param name="remoteIpAddress"></param>
        /// <returns></returns>
        public static async Task<string?> GetAddressByRemoteIpAddress(string remoteIpAddress)
        {
            // 设置API地址
            string apiUrl = $"https://restapi.amap.com/v3/ip?key=b9fe1600bd434cee8375f822b3e0da15&ip={remoteIpAddress}";
            /* 高德地图请求响应
             {
                "status": "1",
                "info": "OK",
                "infocode": "10000",
                "province": "北京市",
                "city": "北京市",
                "adcode": "110000",
                "rectangle": "116.0119343,39.66127144;116.7829835,40.2164962"
              }
             */

            // 创建HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 发送GET请求并获取响应
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // 确保响应成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 解析JSON数据
                    var responseData = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    // 提取city字段
                    string city = responseData.GetProperty("province").GetString();
                    // 输出响应内容
                    Console.WriteLine(responseBody);

                    return city;
                }
                catch (HttpRequestException ex)
                {
                    // 发生HTTP请求错误
                    Console.WriteLine($"HTTP请求错误: {ex.Message}");
                    return null;
                }
                catch (JsonException ex)
                {
                    // JSON解析错误
                    Console.WriteLine($"JSON解析错误: {ex.Message}");
                    return null;
                }
            }
        }

        public static async Task<string?> GetAddressByHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            var addressIp = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return await GetAddressByRemoteIpAddress(addressIp);
        }

        public static long TryGetUserId(HttpContext HttpContext)
        {
            var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdStr, out long userId))
            {
                return userId;
            }
            return default;
        }


        public static long GetUserId(HttpContext HttpContext)
        {
            var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
            {
                throw new ArgumentNullException(nameof(userIdStr), "");
            }
            return long.Parse(userIdStr);
        }



    }
}
