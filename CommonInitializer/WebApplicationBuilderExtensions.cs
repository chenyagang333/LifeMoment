using Chen.Commons;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Chen.JWT;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using Chen.ASPNETCore;
using Chen.Commons.JsonConverters;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;
using FluentValidation.AspNetCore;
using FluentValidation;
using Chen.EventBus;
using StackExchange.Redis;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;

namespace CommonInitializer
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureDbConfiguration(this WebApplicationBuilder builder)
        {
            // Tips:在linux开发环境docker容器中使用MySqlConnection连接mysql容器提示找不到主机
            // 原因是在项目启动时，mysql没有启动，所以要先启动mysql后再启动项目
            string? connStr = builder.Configuration.GetConnectionString($"config_conn");
            builder.Configuration.AddDbConfiguration(
                () => new MySqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
        }

        public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;
            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            services.RunModuleInitializers(assemblies); // 注册项目自己的服务
            if (!string.IsNullOrEmpty(initOptions.ConStrKey))
            {
                services.AddAllDbContexts(ctx =>
                {
                    // 连接字符串如果放在appsetting.json中，会有泄密风险
                    // 如果放在UserSecrets中，每个项目都要配置，很麻烦。
                    // 因此这里推荐放到环境变量中
                    string? connStr = configuration.GetValue<string>(initOptions.ConStrKey);
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));
                    ctx.UseMySql(connStr, serverVersion);
                }, assemblies);
            }

            // 开始：Authentication,Authorization
            // 只需要校验 Authentication 报文头的地方（非IdentityService.WebAPI项目）也需要启动这些
            // IdentityService 项目还需要启动AddIdentityCore
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();

            JWTOptions? jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>();
            builder.Services.AddJWTAuthentication(jwtOpt);

            // 启动 Swagger 中的【Authorize】按钮。这样就不用在每个项目的 AddSwaggerGen 中单独配置了。
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });
            // 结束：Authentication,Authorization

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 注册 MediatR 领域事件是：微服务内部的，进程内的通信。
            services.AddMediatR(cfg =>
            {
                //cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.RegisterServicesFromAssemblies(assemblies.ToArray());
            });
            //// 添加 Filter
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<UnitOfWorkFilter>(); // 添加工作单元，自动执行保存实体
            });
            services.Configure<JsonOptions>(options =>
            {
                // 设置时间格式，而非“2008-08-02T08:08:08”这样的格式
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
            });
            // 注册

            services.AddCors(options =>
            {
                // 更好的再Program.cs中用绑定方式读取配置的方法：https://github.com/dotnet/aspnetcore/issues/21491
                //不过比较麻烦。
                var corsOpt = configuration.GetSection("Cors").Get<CorsSettings>();
                string[] urls = corsOpt.Origins;
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            // 注册日志服务
            services.AddLogging(builder =>
            {
                Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(initOptions.LogFilePath)
                .CreateLogger();
                builder.AddSerilog();
            });
            // 注册模型验证服务
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblies(assemblies);
            //

            services.Configure<JWTOptions>(configuration.GetSection("JWT"));
            services.Configure<IntegrationEventRabbitMQOptions>(configuration.GetSection("RabbitMQ"));
            services.AddEventBus(initOptions.EventBusQueueName, assemblies);


            // Redis 配置
            string? redisConnStr = configuration.GetValue<string>("Redis:ConnStr");
            if (redisConnStr == null)
                throw new ArgumentNullException($"Redis:ConnStr cannot be null");

            //IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
            //services.AddSingleton(typeof(IConnectionMultiplexer),redisConnMultiplexer);

            builder.Services.AddStackExchangeRedisCache(opt =>
            { // 分布式缓存
                opt.Configuration = redisConnStr;
                //opt.InstanceName = "IdentityService_"; // 为 Key 加前缀，防止混乱
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            if (initOptions.ProfileAssemblyMarkerTypes != null)
            {
                // 添加 AddAutoMapper
                builder.Services.AddAutoMapper(initOptions.ProfileAssemblyMarkerTypes);
            }

        }
    }
}
