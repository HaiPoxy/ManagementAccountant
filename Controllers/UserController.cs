using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccountManagermnet.Controllers
{
      
    public class UserController : BaseDataController 
    {
        private readonly AccountDbContext _context;

        private IConfiguration _config;

        public UserController(AccountDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUser(string? search)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.Id.ToString().Contains(search) || g.UserName.Contains(search));

            }
            var userList = await query.OrderBy(x => x.Id)
                         .Select(u => new
                         {
                             u.Id,
                             u.UserName,
                             u.Email,
                             u.Password,
                             Roles = u.UserRoles.Select(ur => ur.Role.RoleName)
                         })
                         .ToListAsync();
           
            return Ok(userList);

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)

        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName);
                if (user != null)
                {
                    // Kiểm tra mật khẩu
                    if (VerifyPassword(loginDTO.Password, user.Password))
                    {
                        var claims = new List<Claim> {
                            new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                            new Claim("id", user.Id.ToString()),
                            new Claim("userName", user.UserName)
                        };
                        var userRoles = _context.UserRoles.Where(u => u.UserId == user.Id).ToList();
                        var roleIds = userRoles.Select(s => s.RoleId).ToList();
                        var roles = _context.Roles.Where(r => roleIds.Contains(r.Id)).ToList();
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim("role", role.RoleName));
                        }
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _config["Jwt:Issuer"],
                            _config["Jwt:Audience"],
                            claims,     
                            expires: DateTime.UtcNow.AddMinutes(10),
                            signingCredentials: signIn);

                        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                        return Ok(jwtToken);
                    }
                    else
                    {
                        return BadRequest("Mật khẩu không đúng");
                    }
                }
                else
                {
                    return NotFound("Người dùng không tồn tại");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // Trong lớp UserController
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateNewUser(UserDTO userDTO)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(p => p.UserName == userDTO.UserName);
                if (existingUser != null)
                {
                    return Conflict(ErrorConst.USER_IS_EXISTS);
                }

                string hashPassword = HashPassword(userDTO.Password);

                var newUser = new User
                {
                    UserName = userDTO.UserName,
                    Email = userDTO.Email,
                    Password = hashPassword,
                };

                // Thêm người dùng mới vào cơ sở dữ liệu
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                foreach (var userRoleDTO in userDTO.UserRoles)
                {
                    var userRole = new UserRole
                    {
                        UserId = newUser.Id,
                        RoleId = userRoleDTO.RoleId, // sử dụng Id của role từ userRoleDTO
                    };
                    _context.UserRoles.Add(userRole);

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userRoleDTO.RoleId);
                    if (role != null)
                    {
                        // Lấy ra tên của role và sử dụng trong logic của bạn
                        var roleName = role.RoleName;
                        // Thêm logic của bạn ở đây
                    }
                }

                return Ok("Thêm thành công");
            }
            catch (Exception)
            {
                return StatusCode(500, $"Lỗi khi tạo phiếu nhập");
            }
        }

        [Authorize(Roles = "Admin, Scrum Master")]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            var existingUser = await _context.Users
                                .Include(x => x.UserRoles)
                                .FirstOrDefaultAsync(g => g.Id == id);
            if (existingUser is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, userDTO.Id));
            }
            string hashPassword = HashPassword(userDTO.Password);

            existingUser.UserName = userDTO.UserName;
            existingUser.Email = userDTO.Email;
            existingUser.Password = hashPassword;

            _context.UserRoles.RemoveRange(existingUser.UserRoles);
            foreach (var userRoleDTO in userDTO.UserRoles)
            {
                existingUser.UserRoles.Add(new UserRole
                {
                    UserId = userRoleDTO.UserId,
                    RoleId = userRoleDTO.RoleId,
                });
            }
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }


        [Authorize(Roles = "Admin, Scrum Master")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
            if (existingUser is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS));

            }
            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }

        #region Private
        // Hàm mã hóa mật khẩu bằng SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Mã hóa mật khẩu
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Chuyển đổi mảng byte sang string hex
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Phương thức để so sánh mật khẩu được cung cấp với mật khẩu đã lưu trong cơ sở dữ liệu
        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                // Mã hóa mật khẩu được cung cấp bằng SHA256
                string hashedInput = HashPassword(password);

                // So sánh mật khẩu đã được mã hóa với mật khẩu đã lưu trong cơ sở dữ liệu
                return hashedInput.Equals(hashedPassword);
            }
        }

        #endregion
    }
}
