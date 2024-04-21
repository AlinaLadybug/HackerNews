using HackerNews.Mappers;
using HackerNews.Models;
using HackerNews.Services;
using HackerNews.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HackerNews.Controllers
{
    [Route("api/story")]
    [ApiController]
    public class StoryController(IStoryService storyService) : Controller
    {
        [HttpGet("top-best/{number}")]
        public async Task<IActionResult> GetTopBestStoriesAsync(int number)
        {
            try
            {

                var stories = await storyService.GetTopBestStoriesAsync(number);
                var storyViewModels = new List<StoryViewModel>();

                foreach (var story in stories)
                {
                    var storyViewModel = StoryMapper.MapToViewModel(story);

                    storyViewModels.Add(storyViewModel);
                }

                return Ok(storyViewModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
