using Moq;
using Nerdy.BLL.Services;
using Nerdy.Domain.Interfaces;
using Nerdy.Domain.Models;
using Nerdy.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerdy.Tests
{
    public class AnnouncementServiceTests
    {
        [Fact]
        public async Task GetAll_ReturnsListOfAnnouncements()
        {
            var mockRepo = new Mock<IAnnouncementRepository>();
            mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Announcement>(new List<Announcement> { new Announcement { Title = "Test",Description = "Test", DateAdded = DateTime.UtcNow } }));
               

            var service = new AnnouncementService(mockRepo.Object);

            var result = await service.GetAllAsync();

            Assert.Single(result);
            Assert.Equal("Test", result.First().Title);
        }

        [Fact]
        public async Task AddAsync_SetsDateAdded_AndCallsRepository()
        {
            var mockRepo = new Mock<IAnnouncementRepository>();
            var service = new AnnouncementService(mockRepo.Object);
            var announcement = new Announcement { Title = "New Announcement", Description = "This is a test announcement." };
            await service.AddAsync(announcement);
            Assert.NotEqual(DateTime.MinValue, announcement.DateAdded);
            mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Announcement>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepository_WithCorrectId()
        {
            var mockRepo = new Mock<IAnnouncementRepository>();
            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

            var service = new AnnouncementService(mockRepo.Object);

            var result = await service.DeleteAsync(1);

            Assert.True(result);
            mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectAnnouncement()
        {
            var mockRepo = new Mock<IAnnouncementRepository>();
            var expectedAnnouncement = new Announcement { Id = 1, Title = "Test Announcement", Description = "Description" };

            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(expectedAnnouncement);

            var service = new AnnouncementService(mockRepo.Object);
            var result = await service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(expectedAnnouncement.Title, result.Title);
            mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepository_WithCorrectData()
        {
            var mockRepo = new Mock<IAnnouncementRepository>();
            var updatedAnnouncement = new Announcement { Id = 1, Title = "Updated Title", Description = "Updated Description" };

            mockRepo.Setup(repo => repo.UpdateAsync(updatedAnnouncement)).ReturnsAsync(true);

            var service = new AnnouncementService(mockRepo.Object);
            var result = await service.UpdateAsync(updatedAnnouncement);

            Assert.True(result);
            mockRepo.Verify(repo => repo.UpdateAsync(updatedAnnouncement), Times.Once);
        }

        [Fact]
        public async Task GetSimilarAsync_ReturnsSimilarAnnouncements()
        {
            var mockRepo = new Mock<IAnnouncementRepository>();
            var targetAnnouncement = new Announcement { Id = 1, Title = "Target Announcement" };

            var similarAnnouncements = new List<Announcement>
            {
                new Announcement { Id = 2, Title = "Similar 1" },
                new Announcement { Id = 3, Title = "Similar 2" }
            };

            mockRepo.Setup(repo => repo.GetSimilarAsync(targetAnnouncement, 3)).ReturnsAsync(similarAnnouncements);

            var service = new AnnouncementService(mockRepo.Object);
            var result = await service.GetSimilarAsync(targetAnnouncement, 3);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            mockRepo.Verify(repo => repo.GetSimilarAsync(targetAnnouncement, 3), Times.Once);
        }
    }
}
