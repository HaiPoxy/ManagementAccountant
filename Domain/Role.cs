//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace AccountManagermnet.Domain
//{
//    public class Role
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }

//        [StringLength(50)]
//        [Required]
//        public string RoleName { get; set; } = " ";

//        public int UserId { get; set; }

//        public User User { get; set; }

//        public static class DefaultRoles
//        {
//                public static readonly List<Role> Roles = new List<Role>
//            {
//                new Role { Id = 1, RoleName = "User" },
//                new Role { Id = 2, RoleName = "Manager" },
//                new Role { Id = 3, RoleName = "Admin" }
//            };
//        }
//    }
//}
