using HackerNews.Models;

namespace HackerNews.Services
{
    public interface IStoryService
    {
        public Task<int[]> GetAllStoriesIdsAsync();
        public Task<Story?> GetStoryByIdAsync(int id);
        public Task<IEnumerable<Story>> GetTopBestStoriesAsync(int number);
    }
}
