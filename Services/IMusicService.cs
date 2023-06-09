using HotMusic.Contract;

namespace HotMusic.Services
{
    public interface IMusicService
    {
        IArtistRepository ArtistRepository { get; }
        IAlbumRepository AlbumRepository { get; }
        ISongRepository SongRepository { get; }

    }
}
