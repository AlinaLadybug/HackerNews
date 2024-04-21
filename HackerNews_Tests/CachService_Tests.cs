using HackerNews.Models;
using HackerNews.Services;
using HackerNews.Services.CashedData;
using HackerNews.Services.Cashing;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task SetDataAsync_Success()
        {
            // Arrange
            var testKey = "testKey";
            var testValue = "testValue";
            _mockDatabase.Setup(db => db.StringSetAsync(testKey, testValue, TimeSpan.FromHours(1), When.NotExists, CommandFlags.None)).ReturnsAsync(true);


            // Act
            var result = await _cachService.SetDataAsync(testKey, testValue, TimeSpan.FromHours(1));


            // Assert
            Assert.True(result);
        }

    }
}
