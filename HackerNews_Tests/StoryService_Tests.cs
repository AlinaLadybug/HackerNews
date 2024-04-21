using HackerNews.Models;
using HackerNews.Services;
using HackerNews.Services.Cashing;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace HackerNews_Tests
{
    public class StoryService_Tests
    {
        private readonly IStoryService _storyService;
        private readonly Mock<ILogger<StoryService>> _mockLogger;
        private readonly HttpClient _httpClient;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ICachService> _mockCacheService;


        public StoryService_Tests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _httpClient.BaseAddress = new Uri("http://test.com/");
            _mockCacheService = new Mock<ICachService>();
            _mockLogger = new Mock<ILogger<StoryService>>();
            _storyService = new StoryService(_mockLogger.Object, _mockCacheService.Object, _httpClient);
        }

        [Fact]
        public async Task GetAllStoriesIdsAsync_Success()
        {
            // Arrange
            var expectedIds = new int[] { 1, 2, 3 };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedIds))
                });

            _mockCacheService.Setup(x => x.GetDataAsync<Story>(It.IsAny<string>())).ReturnsAsync((Story)null);

            // Act
            var result = await _storyService.GetAllStoriesIdsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIds, result);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ReturnsStory_WhenValidIdProvided()
        {
            // Arrange
            var id = 123;
            var expectedStory = new Story { Id = id, Title = "Test Story" };
            _mockCacheService.Setup(x => x.GetDataAsync<Story>(It.IsAny<string>())).ReturnsAsync((Story)null);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedStory)),
                });

            // Act
            var result = await _storyService.GetStoryByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expectedStory, result);
            _mockCacheService
                .Verify(x => x.SetDataAsync($"Story_{id}", JsonConvert.SerializeObject(expectedStory), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ReturnsCachedStory_WhenValidIdProvided()
        {
            // Arrange
            var id = 123;
            var expectedStory = new Story { Id = id, Title = "Test Story" };
            _mockCacheService
                .Setup(x => x.GetDataAsync<Story>(It.IsAny<string>()))
                .ReturnsAsync(expectedStory);

            // Act
            var result = await _storyService.GetStoryByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expectedStory, result);
        }

        [Fact(Skip ="")]
        public async Task GetTopBestStories_Success()
        {
            //Arrange
            var stories = new List<Story>
            {
                new Story{Id = 1, Score = 1000, Title = "Story_HighestScore" },
                new Story{Id = 2, Score = 500, Title = "Story_LowestScore"},
                new Story{Id = 3, Score = 750, Title = "Story"},
            };
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.AbsoluteUri.Contains("beststories.json")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[1, 2, 3]")
                });

            _mockHttpMessageHandler
               .Protected()
               .SetupSequence<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.Is<HttpRequestMessage>(req => !req.RequestUri.AbsoluteUri.Contains("beststories")),
                   ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(stories[0])),
                })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(stories[1])),
                })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(stories[2])),
                });
            //var mockStoryService = new Mock<IStoryService>();
            //mockStoryService.Setup(x => x.GetAllStoriesIdsAsync()).ReturnsAsync([1, 2, 3]);
            //mockStoryService.SetupSequence(x => x.GetStoryByIdAsync(It.IsAny<int>()))
            //           .ReturnsAsync(stories[0])
            //           .ReturnsAsync(stories[1])
            //           .ReturnsAsync(stories[2]);
            var expectedStories = stories.OrderByDescending(x => x.Score);

            //Act
            var result = (await _storyService.GetTopBestStoriesAsync(2)).ToList();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equivalent(stories[0], result[0]);
            Assert.Equivalent(stories[1], result[2]);
        }
    }
}