using HackerNews.Services.Cashing;
using Newtonsoft.Json;
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

                var isSuccess =  await db.StringSetAsync(key, value, expiry);

                return isSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when setting data into redis {Key}, {Value}", key, value);
                return default;
            }
        }
    }
}
