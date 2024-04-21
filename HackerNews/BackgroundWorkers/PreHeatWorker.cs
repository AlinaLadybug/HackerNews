using HackerNews.Services;

namespace HackerNews.BackgroundWorkers
{
    public class PreHeatWorker(ILogger<PreHeatWorker> logger, IStoryService storyService) : BackgroundService
    {
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Background Service to get stories is starting...");

                while (!cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);

                    var ids = await storyService.GetAllStoriesIdsAsync();
                    var tasks = ids.Select(storyService.GetStoryByIdAsync);

                    await Task.WhenAll(tasks);

                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred when worker is getting stories");
            }
        }
    }
}
