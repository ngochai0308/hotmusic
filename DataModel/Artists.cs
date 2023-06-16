using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotMusic.DataModel
{
    [Table("Artist")]
    public class Artists
    {
        [Key]
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string? Avatar { get; set; }
        public string ArtistBio { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
