using System.ComponentModel;

namespace HotMusic.Models
{
    public class CategoryDisplayViewModel
    {
        public int CategoryId { get; set; }
        [DisplayName("Thể loại")]
        public string CategoryTitle { get; set; }
        [DisplayName("Mã quốc gia")]
        public int CountryId { get; set; }
        [DisplayName("Tên quốc gia")]
        public string? CountryTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiledBy { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
