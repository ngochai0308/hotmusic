﻿using HotMusic.Contract;
using HotMusic.DataModel;
using HotMusic.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace HotMusic.Repository
{
    public class SongRepository : ISongRepository
    {
        private MusicDbContext _dbContext;

        public SongRepository(MusicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Songs songs)
        {
            _dbContext.Add(songs);
            //_dbContext.Songs.Add(songs); The same with about command
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var delObj = _dbContext.Songs.Find(id);
            if (delObj != null)
            {
                _dbContext.Remove(delObj);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Songs> GetAll(string keyword = "")
        {
            var query = from s in _dbContext.Songs
                        join a in _dbContext.Artists on s.ArtistId equals a.ArtistId
                        join c in _dbContext.Category on s.CategoryId equals c.CategoryId
                        select new Songs()
                        {
                            ArtistId = a.ArtistId,
                            ArtistName = a.ArtistName,
                            CategoryId = s.CategoryId,
                            CategoryTitle = c.CategoryTitle,
                            SongId = s.SongId,
                            SongTitle = s.SongTitle,
                            SongUrl = s.SongUrl,
                            ViewCount = s.ViewCount,
                            CreatedBy = s.CreatedBy,
                            CreatedDate = s.CreatedDate,
                            ModifiedDate = s.ModifiedDate,
                            ModifiledBy = s.ModifiledBy,
                            Image = s.Image
                        };
            var listResult = query.Where(s => s.SongTitle.Contains(keyword)).ToList();
            return listResult;
        }

        public Songs GetById(int id)
        {
            var query = from s in _dbContext.Songs
                        join a in _dbContext.Artists on s.ArtistId equals a.ArtistId
                        join c in _dbContext.Category on s.CategoryId equals c.CategoryId
                        select new Songs()
                        {
                            ArtistId = a.ArtistId,
                            ArtistName = a.ArtistName,
                            CategoryId = s.CategoryId,
                            CategoryTitle = c.CategoryTitle,
                            SongId = s.SongId,
                            SongTitle = s.SongTitle,
                            SongUrl = s.SongUrl,
                            ViewCount = s.ViewCount,
                            CreatedBy = s.CreatedBy,
                            CreatedDate = s.CreatedDate,
                            ModifiedDate = s.ModifiedDate,
                            ModifiledBy = s.ModifiledBy,
                            Image = s.Image
                        };
            var song = query.FirstOrDefault(s => s.SongId == id);
            return song;
        }

        public void Update(Songs songs)
        {
            _dbContext.Update(songs);
            _dbContext.SaveChanges();
        }
    }
}
