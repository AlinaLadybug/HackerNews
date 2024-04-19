using HackerNews.Models;
using HackerNews.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HackerNews.Controllers
{
    [Route("api/story")]
    [ApiController]
    public class StoryController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly IStoryService _storyService;
        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        [HttpGet("top-best/{number}")]
        public async Task<IEnumerable<Story>> GetTopBestStoriesAsync(int number, CancellationToken cancellationToken)
        {
            var stories = new List<Story>();
            var ids = await _storyService.GetAllStoriesIdsAsync();
            for (int i = 0; i < ids.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                var story = await _storyService.GetStoryByIdAsync(ids[i]);
                if (story != null)
                    stories.Add(story);
            }
            stories.OrderByDescending(story => story.Score);

            return stories.Take(number);
            // save to redis
            //var responseString = await client.GetStringAsync("https://hacker-news.firebaseio.com/v0/beststories.json");
            //var storiesIDs = JsonConvert.DeserializeObject<int[]>(responseString);
            //var stories = new List<Story>();
            //for (int i = 0; i < storiesIDs.Length; i++)
            //{
            //    var id = storiesIDs[i];
            //    var storyDetail = await client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json");
            //    var story = JsonConvert.DeserializeObject<Story>(storyDetail);
            //    stories.Add(story);
            //}


            //return stories;
        }
    }
}
