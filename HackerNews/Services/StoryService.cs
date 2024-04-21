using HackerNews.Models;
using HackerNews.Services.Cashing;
using Newtonsoft.Json;

namespace HackerNews.Services
{
    public class StoryService(ILogger<StoryService> logger, ICachService cachService, HttpClient httpClient) : IStoryService
    {
        public async Task<int[]> GetAllStoriesIdsAsync()
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<int[]>("beststories.json");
                if (response == null)
                {
                    response = [];
                }
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching best story IDs.");
                throw;
            }
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            try
            {
                var key = $"Story_{id}";
                var cachedStory = await cachService.GetDataAsync<Story>(key);
                if (cachedStory != null)
                {
                    return cachedStory;
                }
                var response = await httpClient.GetStringAsync($"item/{id}.json");
                var story = JsonConvert.DeserializeObject<Story>(response);

                await cachService.SetDataAsync(key, response, expiry: TimeSpan.FromDays(1));

                return story;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching story details for ID: {StoryId}", id);

                return default;
            }
        }

        public async Task<IEnumerable<Story>> GetTopBestStoriesAsync(int number)
        {
            try
            {
                var stories = new List<Story>();
                var ids = await GetAllStoriesIdsAsync();
                var tasks = ids.Select(GetStoryByIdAsync);

                await Task.WhenAll(tasks);

                foreach (var item in tasks)
                {
                    if (item.Result != null)
                    {
                        stories.Add(item.Result);
                    }
                }

                return stories.OrderByDescending(story => story.Score).Take(number);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting top best stories");

                throw;
            }
        }
    }
}
