
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhac.DataModel
{
    public class MusicDbContext : DbContext
    {
        public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistSongs>().HasKey(p => new { p.SongId, p.PlaylistId });
            //modelBuilder.Entity<Songs>().HasKey(p => new { p.SongId,p.AlbumId});
        }

        public DbSet<Songs> Songs { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Albums> Albums { get; set; }
        public DbSet<Playlists> Playlists { get; set; }

        public DbSet<PlaylistSongs> PlaylistSongs { get; set; }

        public DbSet<Users>? Users { get; set; }
    }
}
