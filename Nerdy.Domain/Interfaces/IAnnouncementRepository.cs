using Nerdy.Domain.Models;

namespace Nerdy.Domain.Interfaces
{
    public interface IAnnouncementRepository
    {
        public Task<Announcement?> GetByIdAsync(int id);
        public Task<IEnumerable<Announcement>> GetAllAsync();
        public Task AddAsync(Announcement announcement);
        public Task<bool> UpdateAsync(Announcement updated);
        public Task<bool> DeleteAsync(int id);
        public Task<IEnumerable<Announcement>> GetSimilarAsync(Announcement target, int count = 3);
    }
}
