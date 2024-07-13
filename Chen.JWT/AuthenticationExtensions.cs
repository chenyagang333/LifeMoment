using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.JWT
{
    public static class AuthenticationExtensions
    {
        // 向 IServiceCollection 添加 JWT 认证服务。
        // JwtBearerDefaults.AuthenticationScheme 表示用于 JWT Bearer 认证的默认方案。
        public static AuthenticationBuilder AddJWTAuthentication(this IServiceCollection services,JWTOptions jwtOpt)
        {
            return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    // 配置JWT Bearer 认证选项。
                    // 这包括设置令牌参数。
                    x.TokenValidationParameters = new()
                    {
                        // 验证令牌的发行者（Issuer）。
                        ValidateIssuer = true,
                        // 验证令牌的受众（Audience）。
                        ValidateAudience = true,
                        // 验证令牌的生命周期。
                        ValidateLifetime = true,
                        // 验证用于签署令牌的签名密钥。
                        ValidateIssuerSigningKey = true,
                        // 将有效的发行者设置为 jwtOpt.Issuer 提供的值。
                        ValidIssuer = jwtOpt.Issuer,
                        // 将有效的受众设置为 jwtOpt.Audience 提供的值。
                        ValidAudience = jwtOpt.Audience,
                        // 将签名密钥设置为 jwtOpt.Key 提供的值
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.Key))
                    };
                });
        }

    }
}
