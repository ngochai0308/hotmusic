using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Repository
{
    /// <summary>
    /// Album repository
    /// </summary>
    public class AlbumRepository : IAlbumRepository
    {
        private readonly MusicDbContext _dbContext;
        public AlbumRepository(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Albums album)
        {
            _dbContext.Albums.Add(album);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var delObj = _dbContext.Albums.Find(id);
            if (delObj != null)
            {
                _dbContext.Albums.Remove(delObj);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Albums> GetAll(string keyword = "")
        {
            var query = from ab in _dbContext.Albums
                        join at in _dbContext.Artists on ab.ArtistId equals at.ArtistId
                        join c in _dbContext.Category on ab.CategoryID equals c.CategoryId
                        select new Albums()
                        {
                            AlbumId = ab.AlbumId,
                            ArtistId = ab.ArtistId,
                            AlbumTitle = ab.AlbumTitle,
                            CategoryID = c.CategoryId,
                            CategoryTitle = c.CategoryTitle,
                            ArtistName = at.ArtistName,
                            Thumbnail = ab.Thumbnail,
                            CreatedDate = ab.CreatedDate,
                            CreatedBy = ab.CreatedBy,
                            ModifiedDate = ab.ModifiedDate,
                            ModifiledBy = ab.ModifiledBy
                        };
            var listResult = query.Where(a=>a.AlbumTitle.Contains(keyword)).ToList();

            return listResult;
        }

        public Albums GetById(int id)
        {
            var query = from ab in _dbContext.Albums
                        join at in _dbContext.Artists on ab.ArtistId equals at.ArtistId
                        join c in _dbContext.Category on ab.CategoryID equals c.CategoryId
                        select new Albums()
                        {
                            AlbumId = ab.AlbumId,
                            ArtistId = ab.ArtistId,
                            AlbumTitle = ab.AlbumTitle,
                            CategoryID = c.CategoryId,
                            CategoryTitle = c.CategoryTitle,
                            ArtistName = at.ArtistName,
                            Thumbnail = ab.Thumbnail,
                            CreatedDate = ab.CreatedDate,
                            CreatedBy = ab.CreatedBy,
                            ModifiedDate = ab.ModifiedDate,
                            ModifiledBy = ab.ModifiledBy
                        };
            var album = query.FirstOrDefault(a => a.AlbumId == id);
            return album; // ctrl + K + D
        }

        public void Update(Albums album)
        {
            _dbContext.Albums.Update(album);
            _dbContext.SaveChanges();
        }
    }
}
