using HackerNews.Models;
using HackerNews.ViewModels;

namespace HackerNews.Mappers
{
    public class StoryMapper
    {
        public static StoryViewModel MapToViewModel(Story story)
        {
            return new StoryViewModel
            {
                Title = story.Title,
                Uri = story.Url,
                PostedBy = story.By,
                Time = new DateTime(story.Time),
                Score = story.Score,
                CommentCount = story.Descendants
            };
        }
    }
}
