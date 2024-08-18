using CommonInitializer;
using CommonInitializer.ConfigOptions;
using FileService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


// 默认初始化配置
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "FileService.WebAPI",
    LogFilePath = "d:/temp/FileService.log",
    ConStrKey = "FileDb"
});

// Add services to the container.
builder.Services // .AddOptions() // ASP.NET Core 项目中 AddOptions() 不写也行，因为框架一定自动执行了
    .Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMS"));
//.Configure<UpYunStorageOptions>(builder.Configuration.GetSection("FileService.UpYou"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
    app.UseSwagger();
    app.UseSwaggerUI();

//启用静态文件
app.UseStaticFiles();

app.UseChenDefault();

app.MapControllers();

app.Run();
