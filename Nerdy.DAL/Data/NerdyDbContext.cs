using Microsoft.EntityFrameworkCore;
using Nerdy.Domain.Models;

namespace Nerdy.DAL.Data
{
    public class NerdyDbContext : DbContext
    {
        public NerdyDbContext(DbContextOptions<NerdyDbContext> options) : base(options) { }

        public DbSet<Announcement> Announcements { get; set; }
    }
}
