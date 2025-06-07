using Microsoft.EntityFrameworkCore;
using Nerdy.DAL.Data;
using Nerdy.Domain.Interfaces;
using Nerdy.Domain.Models;
using Nerdy.Domain.Utility;

namespace Nerdy.DAL.Repositories
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly NerdyDbContext _context;
        private readonly SimilarWords _similar;

        public AnnouncementRepository(NerdyDbContext context, SimilarWords similar)
        {
            _context = context;
            _similar = similar;
        }

        public async Task AddAsync(Announcement announcement)
        {
            await _context.Announcements.AddAsync(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Announcements.FindAsync(id);
            if (entity == null) return false;

            _context.Announcements.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .OrderByDescending(a => a.DateAdded)
                .ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(int id) => await _context.Announcements.FindAsync(id);


        public async Task<IEnumerable<Announcement>> GetSimilarAsync(Announcement target, int count = 3)
        {
            var words = _similar.GetWords(target.Title).Union(_similar.GetWords(target.Description)).ToHashSet();

            var announcements = await _context.Announcements
            .Where(a => a.Id != target.Id)
            .ToListAsync();

            return announcements
                .Where(a => _similar.GetWords(a.Title)
                .Union(_similar.GetWords(a.Description))
                .Intersect(words).Any())
                .OrderByDescending(a => a.DateAdded)
                .Take(count);
        }

        public async Task<bool> UpdateAsync(Announcement updated)
        {
            var existing = await _context.Announcements.FindAsync(updated.Id);

            if (existing == null) return false;

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
