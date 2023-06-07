using Microsoft.EntityFrameworkCore;

namespace WebsiteMusic.DataModel
{
    public class MusicDBContext : DbContext
    {
        public MusicDBContext(DbContextOptions options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Dinh nghia 2 key trong bang playlistSong
            modelBuilder.Entity<PlaylistSong>().HasKey(p => new { p.SongId, p.PlaylistId });
            modelBuilder.Entity<Favourite>().HasKey(f => new { f.SongId, f.UserId });
        }

        public DbSet<Song> Song { get; set; }
        public DbSet<Album> Album { get; set; }
        public DbSet<Artist> Artist { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<PlaylistSong> PlaylistSong { get; set; }
        public DbSet<Favourite> FavouriteSong { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Country> Country { get; set; }
    }
}
