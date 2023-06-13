using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Repository
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly MusicDbContext _dbContext;

        public PlaylistRepository(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Playlists playlists)
        {
            _dbContext.Playlists.Add(playlists);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var delPlaylist = _dbContext.Playlists.Find(id);
            if (delPlaylist != null)
            {
                _dbContext.Playlists.Remove(delPlaylist);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Playlists> GetAll(string keyword = "")
        {
            var listData = _dbContext.Playlists.ToList();
            return listData;
        }

        public Playlists GetById(int id)
        {
            var onePlaylist = _dbContext.Playlists.Find(id);
            return onePlaylist;
        }

        public void Update(Playlists playlists)
        {
            _dbContext.Playlists.Update(playlists);
            _dbContext.SaveChanges();
        }
    }
}
