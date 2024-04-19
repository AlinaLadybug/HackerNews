using HackerNews.Models;
using HackerNews.Services.Cashing;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HackerNews.Services
{
    public class StoryService : IStoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StoryService> _logger;
        private readonly ICachService _cachService;

        public StoryService(IHttpClientFactory httpClientFactory, ILogger<StoryService> logger,
            ICachService cachService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
            _logger = logger;
            _cachService = cachService;
        }

        public async Task<int[]> GetAllStoriesIdsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<int[]>("beststories.json");
                if (response == null)
                {
                    response = [];
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching best story IDs.");
                throw;
            }
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            try
            {
                var key = $"Story_{id}";
                var cachedStory = await _cachService.GetDataAsync<Story>(key);
                if (cachedStory != null)
                {
                    return cachedStory;
                }

                var response = await _httpClient.GetStringAsync($"item/{id}.json");
                var story = JsonConvert.DeserializeObject<Story>(response);

                await _cachService.SetDataAsync(key, response, expiry: TimeSpan.FromDays(1));

                return story;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching story details for ID: {StoryId}", id);

                return default;
            }
        }
    }
}
