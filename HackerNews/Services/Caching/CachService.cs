using HackerNews.Services.Cashing;
using Newtonsoft.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace HackerNews.Services.CashedData
{
    public class CachService : ICachService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<CachService> _logger;
        public CachService(ILogger<CachService> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {

            //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            //IDatabase db = redis.GetDatabase();

            //var hash = new HashEntry[] {new HashEntry("name", "John"),
            //                            new HashEntry("surname", "Smith"),
            //                            new HashEntry("company", "Redis"),
            //                            new HashEntry("age", "29"),
            //                            };
            //var jsonCommands = db.JSON();
            //var data = await jsonCommands.GetEnumerableAsync<T>(key);
            //return data;
            try
            {
                var db = _connectionMultiplexer.GetDatabase();

                var data = await db.StringGetAsync(key);

                if (!data.IsNullOrEmpty)
                {
                    return JsonConvert.DeserializeObject<T>(data);
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when getting data from redis {Key}", key);

                return default;
            }
        }

        public async Task<bool> SetDataAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                var db = _connectionMultiplexer.GetDatabase();

                return await db.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when setting data into redis {Key}, {Value}", key, value);
                return default;
            }
        }
    }
}
