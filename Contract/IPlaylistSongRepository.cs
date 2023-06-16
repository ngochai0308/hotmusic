using HotMusic.DataModel;

namespace HotMusic.Contract
{
    public interface IPlaylistSongRepository
    {
        IEnumerable<PlaylistSongs> GetAll(string keyword = "");

    }
}
