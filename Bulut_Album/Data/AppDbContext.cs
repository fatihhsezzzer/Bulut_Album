using Bulut_Album.Models;
using Bulut_Album.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bulut_Album.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Media> Media { get; set; }
    }
}
