using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace AccountManagermnet.Domain
{
    public class GoodsReceivedNoteDetail
    {
        [Key]
        [NotNull]
        [StringLength(50)]
        public string GRNDId { get; set; } = "";
        [StringLength(50)]
        public string WarehousId { get; set; } = "";
        public int Quantity { get; set; }
        public int UnitPirce { get; set; }
        [StringLength(50)]
        public string DebitAccount { get; set; } = "";
        [StringLength(50)]
        public string CreditAccount { get; set; } = "";

        //Liên kết One to Many vs GoodsReceivedNote
        public string GRN_Id {  get; set; }
        public GoodsReceivedNote GoodsReceivedNote { get; set; }

        //Liên kết One to Many vs ProductCategory
        public string ProductId { get; set; }
        public ProductCategory ProductCategorys { get; set; }


    }
}
