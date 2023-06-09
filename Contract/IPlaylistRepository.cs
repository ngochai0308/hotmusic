using QuanLyNhac.DataModel;

namespace HotMusic.Contract
{
    public interface IPlaylistRepository
    {
        IEnumerable<Playlists> GetAll(string keyword = "");
        Playlists GetById(int id);
        void Add(Playlists playlists);
        void Update(Playlists playlists);
        void Delete(int id);
    }
}
