using Bulut_Album.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulut_Album.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<UploadLog> UploadLogs { get; set; }
    }
}
