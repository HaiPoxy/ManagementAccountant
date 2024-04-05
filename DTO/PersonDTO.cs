using System.ComponentModel.DataAnnotations;

namespace AccountManagermnet.DTO
{
    public class PersonDTO
    {
        public string Id { get; set; }

        public string PName { get; set; } 
        
        public string Address { get; set; } 
        
        public string PhoneNumber { get; set; } 
        
        public string Email { get; set; }   
        
        public string TaxCode { get; set; }         
    }
}
