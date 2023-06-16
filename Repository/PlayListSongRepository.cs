using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Repository
{
    public class PlayListSongRepository : IPlaylistSongRepository
    {
        private readonly MusicDbContext _dbContext;

        public PlayListSongRepository(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<PlaylistSongs> GetAll(string keyword = "")
        {
            var listData = _dbContext.PlaylistSongs.ToList();
            return listData;
        }
    }
}
