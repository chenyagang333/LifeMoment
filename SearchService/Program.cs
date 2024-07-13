using CommonInitializer;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using SearchService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ƒ¨»œ≥ı ºªØ≈‰÷√
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "SearchService.WebAPI",
    LogFilePath = "d:/temp/SearchService.log"
});


builder.Services.Configure<ElasticSearchOptions>(builder.Configuration.GetSection("ElasticSearch"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();


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
