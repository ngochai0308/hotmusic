using CsvHelper.Configuration;

namespace HotMusic.DataModel
{
    public class AlbumMap : ClassMap<Albums>
    {
        public AlbumMap()
        {
            Map(m => m.AlbumId).Name("Mã Album");
            Map(m => m.AlbumTitle).Name("Tiêu đề");
            Map(m => m.Thumbnail).Name("Ảnh Album");
            Map(m => m.ArtistId).Name("Mã nghệ sĩ");
            Map(m => m.ArtistName).Name("Tên nghệ sĩ");
            Map(m => m.CategoryID).Name("Mã thể loại");
            Map(m => m.CategoryTitle).Name("Tên thể loại");
            Map(m => m.CreatedDate).Name("Ngày tạo");
            Map(m => m.CreatedBy).Name("Người tạo");
            Map(m => m.ModifiedDate).Name("Ngày thay đổi");
            Map(m => m.ModifiledBy).Name("Người thay đổi");
        }
    }
}
