using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AccountManagermnet.Domain
{
    public class Person
    {
        [Key]
        [NotNull] 
        public string Id { get; set; } = "";
        [StringLength(50)]
        public string PName { get; set; } = "";
        [StringLength(50)]
        public string Address { get; set; } = "";
        [StringLength(50)]
        public string PhoneNumber { get; set; } = "";
        [StringLength(50)]
        public string Email { get; set; } = "";
        [StringLength(50)]
        public string TaxCode { get; set; } = "";
        
        //Reference GRN
        public GoodsReceivedNote GoodsReceivedNote { get; set; } 

        

    }
}
