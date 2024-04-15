namespace AccountManagermnet.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<RoleDTO> Roles { get; set; }
    }

}
