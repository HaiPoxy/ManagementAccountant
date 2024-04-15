//using System.ComponentModel.DataAnnotations;
//using System.Diagnostics.CodeAnalysis;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AccountManagermnet.Domain
{
    public class GoodsReceivedNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GRNId { get; set; }


        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DocumentDay { get; set; }

        [StringLength(50)]
        public string DocumentNumber { get; set; } = "";

        [StringLength(100)]
        public string Detail { get; set; } = "";

        public string PersonID { get; set; } 
        //liên kết 1 to 1 với bảng Person
        public Person Person { get; set; }

        //liên kết 1 to many với bảng GoodsReceivedNoteDetail
        public ICollection<GoodsReceivedNoteDetail> GoodsReceivedNoteDetails { get; set; }
    }
}
