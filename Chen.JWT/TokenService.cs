using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chen.JWT
{
    public class TokenService : ITokenService
    {
        public string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
        {
            // 使用传入的 ExpireSeconds 参数创建 TimeSpan 对象，表示令牌的过期时长
            TimeSpan ExpiryDuration = TimeSpan.FromSeconds(options.ExpireSeconds);

            // 使用传入的 Key 创建对称安全密钥对象
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));

            // 使用安全密钥对象创建签名凭据，指定算法为 HmacSha256Signature
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 创建 JwtSecurityToken 对象，指定发行人 Issuer、受众 Audience、声明 claims、过期时间 expires、签名凭据 credentials
            var tokenDescriptor = new JwtSecurityToken(
                issuer:options.Issuer,
                audience:options.Audience,
                claims:claims,
                expires:DateTime.Now.Add(ExpiryDuration),
                signingCredentials: credentials
                );

            // 使用 JwtSecurityTokenHandler 对象将 JwtSecurityToken 转换为字符串格式的 JWT 令牌并返回
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
