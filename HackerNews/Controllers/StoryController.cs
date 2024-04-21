using HackerNews.Models;
using HackerNews.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Controllers
{
    [Route("api/story")]
    [ApiController]
    public class StoryController(IStoryService storyService) : Controller
    {
        [HttpGet("top-best/{number}")]
        public async Task<IEnumerable<Story>> GetTopBestStoriesAsync(int number)
        {
            return await storyService.GetTopBestStoriesAsync(number);
        }
    }
}
