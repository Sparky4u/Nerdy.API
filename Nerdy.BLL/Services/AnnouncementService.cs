using Nerdy.Domain.Interfaces;
using Nerdy.Domain.Models;

namespace Nerdy.BLL.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        public readonly IAnnouncementRepository _repository;

        public AnnouncementService(IAnnouncementRepository repository)
        {
            _repository = repository;
        }

        public Task AddAsync(Announcement announcement)
        {
            announcement.DateAdded = DateTime.UtcNow;
            return _repository.AddAsync(announcement);
        }

        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
        public Task<IEnumerable<Announcement>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Announcement?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<IEnumerable<Announcement>> GetSimilarAsync(Announcement target, int count = 3) => _repository.GetSimilarAsync(target, count);
        public Task<bool> UpdateAsync(Announcement updated) => _repository.UpdateAsync(updated);
    }
}
