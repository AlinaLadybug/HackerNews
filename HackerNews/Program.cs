using HackerNews.Services;
using HackerNews.Services.Caching;
using HackerNews.Services.CashedData;
using HackerNews.Services.Cashing;
using Newtonsoft.Json.Converters;
using Redis.OM;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IConnectionMultiplexer>
    (ConnectionMultiplexer.Connect(builder.Configuration["REDIS_CONNECTION_STRING"]));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.Converters.Add(new StringEnumConverter()));

builder.Services.AddSwaggerGenNewtonsoftSupport();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO
builder.Services.AddTransient<ICachService, CachService>();
builder.Services.AddTransient<IStoryService, StoryService>();
builder.Services.AddHostedService<Worker>();
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
