namespace HackerNews.Services.Cashing
{
    public interface ICachService
    {
        public Task<T?> GetDataAsync<T>(string key);
        public Task<bool> SetDataAsync(string key, string value, TimeSpan? expiry);

    }
}
