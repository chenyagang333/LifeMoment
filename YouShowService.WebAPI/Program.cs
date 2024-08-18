using Chen.JWT;
using CommonInitializer;
using CommonInitializer.ConfigOptions;
using Microsoft.EntityFrameworkCore;
using YouShowService.Domain.Options;
using YouShowService.WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ƒ¨»œ≥ı ºªØ≈‰÷√
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "YouShowService.WebAPI",
    LogFilePath = "d:/temp/YouShowService.log",
    ProfileAssemblyMarkerTypes = [typeof(CustomProfile)],
    ConStrKey = "DefaultDb"

});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection("FileService:Endpoint"));
builder.Services.Configure<SearchServiceOptions>(builder.Configuration.GetSection("SearchService"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseChenDefault();

app.MapControllers();

app.Run();
