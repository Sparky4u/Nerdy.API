using Microsoft.EntityFrameworkCore;
using Nerdy.DAL.Data;
using Nerdy.DAL.Repositories;
using Nerdy.Domain.Models;
using Nerdy.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerdy.Tests.TestDb
{
    public class TestDbContext : IDisposable
    {
        public NerdyDbContext Context { get; }

        public TestDbContext()
        {
            var options = new DbContextOptionsBuilder<NerdyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestNerdyDb")
                .Options;

            Context = new NerdyDbContext(options);
        }

        public void Dispose() => Context.Dispose();

        [Fact]
        public async Task AddAsync_SavesAnnouncement()
        {
            using var db = new TestDbContext();
            var repo = new AnnouncementRepository(db.Context, new SimilarWords());

            var announcement = new Announcement { Title = "New Announcement", Description = "Test Description" };

            await repo.AddAsync(announcement);

            var result = await db.Context.Announcements.FirstOrDefaultAsync(a => a.Title == "New Announcement");

            Assert.NotNull(result);
            Assert.Equal("New Announcement", result.Title);
        }

        [Fact]
        public async Task DeleteAsync_RemovesAnnouncement()
        {
            using var db = new TestDbContext();
            var repo = new AnnouncementRepository(db.Context, new SimilarWords());

            var announcement = new Announcement { Title = "To Delete" };
            await db.Context.Announcements.AddAsync(announcement);
            await db.Context.SaveChangesAsync();

            var result = await repo.DeleteAsync(announcement.Id);

            Assert.True(result);
            Assert.Null(await db.Context.Announcements.FindAsync(announcement.Id));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAnnouncementsOrderedByDate()
        {
            using var db = new TestDbContext();
            db.Context.Database.EnsureDeleted(); 
            db.Context.Database.EnsureCreated(); 


            var repo = new AnnouncementRepository(db.Context, new SimilarWords());

            var announcements = new List<Announcement>
            {   
                new Announcement { Title = "Old", DateAdded = DateTime.UtcNow.AddDays(-1) },
                new Announcement { Title = "New", DateAdded = DateTime.UtcNow }
            };

            await db.Context.Announcements.AddRangeAsync(announcements);
            await db.Context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Equal("New", result.First().Title); 
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectAnnouncement()
        {
            using var db = new TestDbContext();
            var repo = new AnnouncementRepository(db.Context, new SimilarWords());

            var announcement = new Announcement { Title = "Find Me" };
            await db.Context.Announcements.AddAsync(announcement);
            await db.Context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(announcement.Id);

            Assert.NotNull(result);
            Assert.Equal("Find Me", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAnnouncement()
        {
            using var db = new TestDbContext();
            var repo = new AnnouncementRepository(db.Context, new SimilarWords());

            var announcement = new Announcement { Title = "Initial Title", Description = "Initial Description" };
            await db.Context.Announcements.AddAsync(announcement);
            await db.Context.SaveChangesAsync();

            announcement.Title = "Updated Title";
            announcement.Description = "Updated Description";

            var result = await repo.UpdateAsync(announcement);

            Assert.True(result);
            var updated = await db.Context.Announcements.FindAsync(announcement.Id);
            Assert.Equal("Updated Title", updated.Title);
        }

        [Fact]
        public async Task GetSimilarAsync_ReturnsSimilarAnnouncements()
        {
            using var db = new TestDbContext();
            var repo = new AnnouncementRepository(db.Context, new SimilarWords());

            var baseAnnouncement = new Announcement { Title = "Apple iPhone", Description = "New device" };
            var similarAnnouncement = new Announcement { Title = "Apple MacBook", Description = "Laptop from Apple" };
            var unrelatedAnnouncement = new Announcement { Title = "Samsung Galaxy", Description = "Android phone" };

            await db.Context.Announcements.AddRangeAsync(baseAnnouncement, similarAnnouncement, unrelatedAnnouncement);
            await db.Context.SaveChangesAsync();

            var result = await repo.GetSimilarAsync(baseAnnouncement, 3);

            Assert.Contains(similarAnnouncement, result);
            Assert.DoesNotContain(unrelatedAnnouncement, result);
        }
    }
}
