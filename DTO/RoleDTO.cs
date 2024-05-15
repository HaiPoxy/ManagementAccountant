namespace AccountManagermnet.DTO
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public List<UserRoleDTO> UserRoles { get; set; }

    }
}
