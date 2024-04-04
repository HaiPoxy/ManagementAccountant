using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AccountManagermnet.DTO
{
    public class AccountCategory
    {
        [Key]
        [NotNull]
        public string Id { get; set; } = "";

        [StringLength(50)]
        public string Name { get; set; } = "";

        [StringLength(50)]
        public string BankAccount { get; set; } = "";

        [StringLength(50)]
        public string BankName { get; set; } = "";

        [StringLength(50)]
        public string ParentId { get; set; } = "";

    }
}
