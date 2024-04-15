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
        public string Name { get; set; } = " ";
        [StringLength(50)]
        public string Email { get; set; } = " ";

        public ICollection<Role> Roles { get; set; }
    }
}
