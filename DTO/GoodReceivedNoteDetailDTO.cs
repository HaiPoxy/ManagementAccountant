using AccountManagermnet.Domain;
using System.ComponentModel.DataAnnotations;

namespace AccountManagermnet.DTO
{
    public class GoodReceivedNoteDetailDTO
    {
        public string GRNDId { get; set; } 
        public string WarehousId { get; set; } 
        public int Quantity { get; set; }
        public int UnitPirce { get; set; }
        public string DebitAccount { get; set; } 
        public string CreditAccount { get; set; } 

        //Liên kết One to Many vs GoodsReceivedNote
        public string GRN_Id { get; set; }

        //Liên kết One to Many vs ProductCategory
        public string ProductId { get; set; }
    }
}
