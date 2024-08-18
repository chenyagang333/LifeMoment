using CommonInitializer;
using CommonInitializer.ConfigOptions;
using FileService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


// Ĭ�ϳ�ʼ������
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "FileService.WebAPI",
    LogFilePath = "d:/temp/FileService.log",
    ConStrKey = "FileDb"
});

// Add services to the container.
builder.Services // .AddOptions() // ASP.NET Core ��Ŀ�� AddOptions() ��дҲ�У���Ϊ���һ���Զ�ִ����
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

//���þ�̬�ļ�
app.UseStaticFiles();

app.UseChenDefault();

app.MapControllers();

app.Run();
