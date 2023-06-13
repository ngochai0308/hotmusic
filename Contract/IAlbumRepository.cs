using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface IAlbumRepository
    {
        IEnumerable<Albums> GetAll(string keyword = "");
        Albums GetById(int id);
        void Add(Albums album);
        void Update(Albums album);
        void Delete(int id);
    }
}
