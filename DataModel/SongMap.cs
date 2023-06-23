using CsvHelper.Configuration;

namespace HotMusic.DataModel
{
    public class SongMap : ClassMap<Songs>
    {
        public SongMap() {
            Map(s => s.SongId).Name("Mã bài hát");
            Map(s => s.SongTitle).Name("Tên bài hát");
            Map(s => s.Image).Name("Ảnh");
            Map(s => s.ArtistId).Name("Mã nghệ sĩ");
            Map(s => s.ArtistName).Name("Tên nghệ sĩ");
            Map(s => s.CategoryId).Name("Mã thể loại");
            Map(s => s.CategoryTitle).Name("Tên thể loại");
            Map(s => s.CreatedDate).Name("Ngày tạo");
            Map(s => s.CreatedBy).Name("Người tạo");
            Map(s => s.ModifiedDate).Name("Ngày thay đổi");
            Map(s => s.ModifiledBy).Name("Người thay đổi");

        }
    }
}
