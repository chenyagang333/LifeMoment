using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Chen.JWT
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        /// 为 Swagger 增加 Authentication 报文头
        /// </summary>
        /// <param name="c">SwaggerGenOptions 实例</param>
        public static void AddAuthenticationHeader(this SwaggerGenOptions c)
        {
            // 添加安全定义，用于描述认证报文头的格式和示例
            c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
            {
                // 说明认证报文头的描述
                Description = "Authorization header. \r\nExample: 'Bearer 12345abcdef'",
                // 认证报文头的名称
                Name = "Authorization",
                // 认证报文头的位置为 Header 
                In = ParameterLocation.Header,
                // 认证方式为 ApiKey
                Type = SecuritySchemeType.ApiKey,
                // 认证的 Scheme 为 "Authorization"
                Scheme = "Authorization"
            });

            // 添加安全要求，指定 Swagger 文档中的接口需要使用该认证方式
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Authorization" // 引用上面定义的安全定义 Id。
                        },
                        Scheme = "oauth2", // 认证的 Scheme 为 "oauth2"
                        Name = "Authorization", // 认证报文头的名称
                        In = ParameterLocation.Header // 认证报文头的位置为 Header
                    },
                    new List<string>() // 要求认证
                }
            });
        }
    }
}
