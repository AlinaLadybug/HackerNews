using HackerNews.Services;

namespace HackerNews_Tests
{
    [TestClass]
    public class StoryServiceTests
    {
        private IStoryService _storyService;

        [TestMethod]
        public async Task GetStoryDetails_ReturnsStoryDetails()
        {
            // Arrange
            int storyId = 12345; // Provide a valid story ID

            // Act
            var storyDetails = await _storyService.GetStoryByIdAsync(storyId);

            // Assert
            Assert.IsNotNull(storyDetails);
            Assert.AreEqual(storyId, storyDetails.Id);
        }
    }
}