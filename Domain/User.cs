using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccountManagermnet.Domain
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string UserName { get; set; } = "";
        [StringLength(50)]
        public string Email { get; set; } = "";
        [StringLength(500)]
        public string Password { get; set; } = "";

        public IList<UserRole> UserRoles { get; set; }
    }
}
