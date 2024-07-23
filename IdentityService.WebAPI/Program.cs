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

// Ĭ�ϳ�ʼ������
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
// AddSignalR �ķֲ�ʽ����ͨ��΢���ṩ�� AddStackExchangeRedis ʵ�֣��������֮���ͨ��
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

#region ���ݿ����� �� Identity�������

builder.Services.AddDataProtection();
// ��¼��ע�����Ŀ����Ҫ����WebApplicationBuilderExtensions�еĳ�ʼ��֮�⣬��Ҫ���µĳ�ʼ��
// ��Ҫ��AddIdentity��������AddIdentityCore
// ��Ϊ��AddIdentity�ᵼ��JWT���Ʋ������ã�AddJwtBearer�лص����ᱻִ�У��������AuthenticationУ��ʧ��
//https://github.com/aspnet/Identity/issues/1376
builder.Services.AddIdentityCore<User>(opt =>
{
    //opt.Lockout.MaxFailedAccessAttempts = 10;
    //opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1000);
    opt.Password.RequireDigit = false;      // �Ƿ�����������
    opt.Password.RequiredLength = 6;        // ��С����
    opt.Password.RequireLowercase = false;  // �Ƿ�������Сд
    opt.Password.RequireNonAlphanumeric = false;  // �Ƿ������������֡�����ĸ���ַ�
    opt.Password.RequireUppercase = false;  // �Ƿ���������д��ĸ
    //�����趨RequireUniqueEmail��������������Ϊ��
    //options.User.RequireUniqueEmail = true;
    opt.User.AllowedUserNameCharacters = ""; // ���ַ���(�� null)�������û�������Ϊ�����ַ�

    //�������У���GenerateEmailConfirmationTokenAsync��֤������
    // ����ǰ��������ӷ��͵��û����䣬�ǾͲ������ã��������ɵ���֤��̫��̫���ӣ������Ҫ�û����룬����Ҫ���ã������Ͷ���
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
