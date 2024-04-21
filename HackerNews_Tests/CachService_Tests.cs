using HackerNews.Services.CashedData;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HackerNews_Tests
{
    public class CachService_Tests
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<ILogger<CachService>> _mockLogger;
        private readonly CachService _cachService;

        public CachService_Tests()
        {
            _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();
            _mockLogger = new Mock<ILogger<CachService>>();
            _mockConnectionMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                                      .Returns(_mockDatabase.Object);

            _cachService = new CachService(_mockLogger.Object, _mockConnectionMultiplexer.Object);
        }

        [Fact]
        public async Task GetDataAsync_WithData_ReturnsDeserializedData()
        {
            // Arrange
            var testData = new { Id = 1, Title = "Test" };
            var serializedTestData = JsonConvert.SerializeObject(testData);
            _mockDatabase.Setup(db => db.StringGetAsync("testKey", CommandFlags.None)).ReturnsAsync(serializedTestData);


            // Act
            var result = await _cachService.GetDataAsync<object>("testKey");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serializedTestData, JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task GetDataAsync_WithException_ReturnsDefault()
        {
            // Arrange

            _mockDatabase.Setup(db => db.StringGetAsync("testKey", CommandFlags.None)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _cachService.GetDataAsync<object>("testKey");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetDataAsync_Failure()
        {
            // Arrange
            var testKey = "testKey";
            var testValue = "testValue";
            var expiry = TimeSpan.FromMinutes(10);

            _mockDatabase.Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);

            // Act
            var result = await _cachService.SetDataAsync(testKey, testValue, expiry);

            // Assert
            Assert.False(result);
        }

    }
}
