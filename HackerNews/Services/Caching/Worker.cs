
namespace HackerNews.Services.Caching
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStoryService _storyService;

        public Worker(ILogger<Worker> logger, IStoryService storyService)
        {
            _logger = logger;
            _storyService = storyService;
        }
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Background Service to get stories is starting...");

                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTime.UtcNow);

                    var ids = await _storyService.GetAllStoriesIdsAsync();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        await _storyService.GetStoryByIdAsync(ids[i]);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when worker is getting stories");
            }
        }
    }
}
