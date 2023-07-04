using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("Lyric")]
    public class Lyric
    {
        [Key]
        public int Id { get; set; }
        public string lyricc { get; set; }
        public int SongId { get; set; }
        [NotMapped]
        public string SongTitle { get; set; }
    }
}
