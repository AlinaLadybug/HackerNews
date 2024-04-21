using HackerNews.Services.Cashing;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HackerNews.Services.CashedData
{
    public class CachService(ILogger<CachService> logger, IConnectionMultiplexer connectionMultiplexer) : ICachService
    {
        public async Task<T?> GetDataAsync<T>(string key)
        {
            try
            {
                var db = connectionMultiplexer.GetDatabase();

                var data = await db.StringGetAsync(key);

                return data.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured when getting data from redis {Key}", key);

                return default;
            }
        }

        public async Task<bool> SetDataAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                var db = connectionMultiplexer.GetDatabase();

                return await db.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured when setting data into redis {Key}, {Value}", key, value);

                return default;
            }
        }
    }
}
