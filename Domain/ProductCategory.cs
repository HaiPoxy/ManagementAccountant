using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AccountManagermnet.Domain
{
    public class ProductCategory
    {
        [Key]
        [NotNull]
        public string Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = "";
        [StringLength(50)]
        public string Unit { get; set; } = "";
        [StringLength(50)]
        public string RevenueAcc { get; set; } = ""; //tài khoản doanh thu
        [StringLength(50)]
        public string GoodsAcc { get; set; } = ""; // tài khoản hàng hóa
        [StringLength(50)]
        public string GOGSAcc { get; set; } = ""; // Cost of Goods Sold Account - tài khoản giá vốn

        //Liên kết Many to One vs GoodsReceivedNoteDetail
        public ICollection<GoodsReceivedNoteDetail> GoodsReceivedNoteDetails { get; set; }

    }
}
