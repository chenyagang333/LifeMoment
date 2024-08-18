using CommonInitializer;
using CommonInitializer.ConfigOptions;
using StackExchange.Redis;
using UserChatService.Domain.Model;
using UserChatService.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 默认初始化配置
builder.ConfigureDbConfiguration();
var initializerOptions = new InitializerOptions
{
    EventBusQueueName = "UserChatService.WebAPI",
    LogFilePath = "d:/temp/UserChatService.log",
    ProfileAssemblyMarkerTypes = [typeof(UserChatProfile)],
    ConStrKey = "UserChatDb",
    SignalRMapHubPattern = "/UserChatHub"
};
builder.ConfigureExtraServices(initializerOptions);

string redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnStr")!;
//builder.Services.AddSignalR();
// AddSignalR 的分布式部署，通过微软提供的 AddStackExchangeRedis 实现，多个服务之间的通信
builder.Services.AddSignalR().AddStackExchangeRedis(redisConnStr, options =>
{
    options.Configuration.ChannelPrefix = RedisChannel.Literal("UserChat_");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<UserChatHub>(initializerOptions.SignalRMapHubPattern);

app.UseChenDefault();

app.MapControllers();

app.Run();
