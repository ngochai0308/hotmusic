using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface IArtistRepository
    {
        IEnumerable<Artist> GetAll(string keyword = "");
        Artist GetById(int id);
        void Add(Artist artist);
        void Update(Artist artist);
        void Delete(int id);
    }
}
