using lab1.Data;
using Microsoft.EntityFrameworkCore;

namespace lab1.Models
{
	public class GalleryDbContext : DbContext
	{
		public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
		{
		}

		public DbSet<ArtistDto> Artist { get; set; }
		public DbSet<PaintingDto> Painting { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PaintingDto>().HasOne(p => p.Author).WithMany(a => a.Works);
		}
	}
}