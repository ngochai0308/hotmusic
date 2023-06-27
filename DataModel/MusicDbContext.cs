using Microsoft.EntityFrameworkCore;

namespace HotMusic.DataModel
{
    public class MusicDbContext : DbContext
    {
        public MusicDbContext(DbContextOptions options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Dinh nghia 2 key trong bang playlistSong
            modelBuilder.Entity<PlaylistSongs>().HasKey(p => new { p.SongId, p.PlaylistId });
            modelBuilder.Entity<Favourite>().HasKey(f => new { f.SongId, f.UserId });
            modelBuilder.Entity<AlbumSongs>().HasKey(f => new { f.SongId, f.AlbumId });
        }

        public DbSet<Songs> Songs { get; set; }
        public DbSet<Albums> Albums { get; set; }
        public DbSet<Artists> Artists { get; set; }
        public DbSet<Playlists> Playlists { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<PlaylistSongs> PlaylistSongs { get; set; }
        public DbSet<Favourite> FavouriteSong { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Lyric> Lyric { get; set; }
        public DbSet<AlbumSongs> AlbumSongs { get; set; }
    }
}
