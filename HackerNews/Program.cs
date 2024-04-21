using HackerNews.BackgroundWorkers;
using HackerNews.Services;
using HackerNews.Services.CashedData;
using HackerNews.Services.Cashing;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();

builder.Services.AddHttpClient<IStoryService, StoryService>((provider, client) =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");

});

builder.Services.AddSingleton<IConnectionMultiplexer>
    (ConnectionMultiplexer.Connect(builder.Configuration["REDIS_CONNECTION_STRING"] ?? throw new ArgumentException("REDIS_CONNECTION_STRING")));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.Converters.Add(new StringEnumConverter()));

builder.Services.AddSwaggerGenNewtonsoftSupport();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO exception handler
builder.Services.AddTransient<ICachService, CachService>();
builder.Services.AddHostedService<PreHeatWorker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
