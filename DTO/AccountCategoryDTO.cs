using System.ComponentModel.DataAnnotations;

namespace AccountManagermnet.DTO
{
    public class AccountCategoryDTO
    {
        public string Id { get; set; } 
        public string Name { get; set; } 
        public string BankAccount { get; set; } 
        public string BankName { get; set; } 
        public string ParentId { get; set; } 
    }
}
