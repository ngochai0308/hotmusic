using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Repository
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly MusicDbContext _dbContext;
        public ArtistRepository(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Add artist
        /// </summary>
        /// <param name="artist">Object contains aritst information</param>
        public void Add(Artists artist)
        {
            _dbContext.Artists.Add(artist);
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// Delete artist by id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var delObj = _dbContext.Artists.Find(id);
            if (delObj != null)
            {
                _dbContext.Artists.Remove(delObj);
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get all artists filter by keyword
        /// </summary>
        /// <param name="keyword">Value to filter</param>
        /// <returns>List data of artist</returns>
        public IEnumerable<Artists> GetAll(string keyword = "")
        {
            var listData = _dbContext.Artists.Where(a => a.ArtistName.Contains(keyword)).ToList();
            return listData;
        }

        /// <summary>
        /// Get artist by Id
        /// </summary>
        /// <param name="id">Artists id</param>
        /// <returns></returns>
        public Artists GetById(int id)
        {
            var artist = _dbContext.Artists.FirstOrDefault(a => a.ArtistId == id);
            return artist;
        }

        /// <summary>
        /// Cập nhật thông tin nghệ sĩ
        /// </summary>
        /// <param name="artist">Đối tượng chứa thông tin nghệ sĩ</param>

        public void Update(Artists artist)
        {
            _dbContext.Artists.Update(artist);
            _dbContext.SaveChanges();
        }
        public void CheckTrackedArtist(int id)
        {
            var trackedArtist = _dbContext.Artists.FirstOrDefault(a => a.ArtistId == id);
            if (trackedArtist != null)
                _dbContext.Entry(trackedArtist).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }
    }
}
