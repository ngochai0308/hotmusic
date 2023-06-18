using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface ISongRepository
    {
        IEnumerable<Songs> GetAll(string keyword = "");
        Songs GetById(int id);
        void Add(Songs songs);
        void Update(Songs songs);
        void Delete(int id);
    }
}
