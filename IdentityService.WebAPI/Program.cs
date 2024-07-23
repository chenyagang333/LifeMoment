using CommonInitializer;
using IdentityService.Domain.DTO.ACustomProfile;
using IdentityService.Domain.Entities;
using IdentityService.Domain.IRespository;
using IdentityService.Domain.IService;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Hubs;
using IdentityService.Infrastructure.Respository;
using IdentityService.Infrastructure.Service;
using IdentityService.WebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 默认初始化配置
builder.ConfigureDbConfiguration();
var initializerOptions = new InitializerOptions
{
    EventBusQueueName = "IdentityService.WebAPI",
    LogFilePath = "d:/temp/IdentityService.log",
    ProfileAssemblyMarkerTypes = [typeof(CustomProfile), typeof(UserChatProfile)],
    ConStrKey = "IdentityDb",
    SignalRMapHubPattern = "/UserChatHub"

};
builder.ConfigureExtraServices(initializerOptions);
string redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnStr")!;
//builder.Services.AddSignalR();
// AddSignalR 的分布式部署，通过微软提供的 AddStackExchangeRedis 实现，多个服务之间的通信
builder.Services.AddSignalR().AddStackExchangeRedis(redisConnStr, options =>
{
    options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalR_");
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
    //c.AddAuthenticationHeader();
});

#region 数据库连接 和 Identity框架配置

builder.Services.AddDataProtection();
// 登录、注册的项目除了要启用WebApplicationBuilderExtensions中的初始化之外，还要如下的初始化
// 不要用AddIdentity，而是用AddIdentityCore
// 因为用AddIdentity会导致JWT机制不起作用，AddJwtBearer中回调不会被执行，因此总是Authentication校验失败
//https://github.com/aspnet/Identity/issues/1376
builder.Services.AddIdentityCore<User>(opt =>
{
    //opt.Lockout.MaxFailedAccessAttempts = 10;
    //opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1000);
    opt.Password.RequireDigit = false;      // 是否必须包含数字
    opt.Password.RequiredLength = 6;        // 最小长度
    opt.Password.RequireLowercase = false;  // 是否必须包含小写
    opt.Password.RequireNonAlphanumeric = false;  // 是否必须包含非数字、非字母的字符
    opt.Password.RequireUppercase = false;  // 是否必须包含大写字母
    //不能设定RequireUniqueEmail，否则不允许邮箱为空
    //options.User.RequireUniqueEmail = true;
    opt.User.AllowedUserNameCharacters = ""; // 空字符串(或 null)，代表用户名允许为任意字符

    //以下两行，把GenerateEmailConfirmationTokenAsync验证码缩短
    // 如果是把重置链接发送到用户邮箱，那就不用配置，不过生成的验证码太长太复杂，如果需要用户输入，则需要配置，这样就短了
    opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});
IdentityBuilder identityBuilder = new(typeof(User), typeof(IdentityService.Domain.Entities.Role), builder.Services);
identityBuilder.AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders()
    //.AddRoleValidator<RoleValidator<Role>>()
    .AddUserManager<IdUserManager>().
    AddRoleManager<RoleManager<IdentityService.Domain.Entities.Role>>();

#endregion


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, MockEmailSender>();
    builder.Services.AddScoped<ISmsSender, MockSmsSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, SendCloudEmailSender>();
    builder.Services.AddScoped<ISmsSender, SendCloudSmsSender>();
}

var app = builder.Build();
//app.Urls.Add("http://localhost:7070");
//app.Urls.Add("http://localhost:5115");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseChenDefault();

app.MapHub<UserChatHub>(initializerOptions.SignalRMapHubPattern);

app.MapControllers();
app.Run();
