using HotMusic.Contract;
using HotMusic.Repository;
using QuanLyNhac.DataModel;

namespace HotMusic.Services
{
    public class MusicService : IMusicService
    {
        private readonly MusicDbContext _dbContext;
        public MusicService(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IArtistRepository _artistRepository;
        public IArtistRepository ArtistRepository
        {
            get
            {
                if (_artistRepository == null)
                {
                    _artistRepository = new ArtistRepository(_dbContext);
                }
                return _artistRepository;
            }
        }

        private IAlbumRepository _albumRepository;
        public IAlbumRepository AlbumRepository
        {
            get
            {
                _albumRepository ??= new AlbumRepository(_dbContext);
                return _albumRepository;
            }
        }

        private ISongRepository _songRepository;
        public ISongRepository SongRepository
        {
            get
            {
                if (_songRepository == null)
                {
                    _songRepository = new SongRepository(_dbContext);
                }
                return _songRepository;
            }
        }
    }
}
