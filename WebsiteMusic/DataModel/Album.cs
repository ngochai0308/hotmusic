using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteMusic.DataModel
{
    [Table("Album")]
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public int CategoryID { get; set; }
    }
}
