using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface IArtistRepository
    {
        IEnumerable<Artists> GetAll(string keyword = "");
        Artists GetById(int id);
        void Add(Artists artist);
        void Update(Artists artist);
        void Delete(int id);
    }
}
