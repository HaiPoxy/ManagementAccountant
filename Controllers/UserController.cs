using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Authorization;
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
        
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUser(string? search, int offset, int limit)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.Id.ToString().Contains(search));

            }
            var userList = await query.OrderBy(x => x.Id)
                                     .Skip(offset)
                                     .Take(limit)
                                     .ToListAsync();
            var pageResult = new PageResult<User>(offset, limit, 0, 0, userList);
            pageResult.Pos = offset;
            pageResult.total_count = 0;
            if (offset == 0)
            {
                pageResult.total_count = await query.CountAsync();
            }
            return Ok(pageResult);

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName);
            if (user == null)
            {
                return NotFound("Tài khoản hoặc mật khẩu không đúng!");
            }

            // Mã hóa mật khẩu được cung cấp và so sánh với mật khẩu đã lưu trong cơ sở dữ liệu
            if (!VerifyPassword(loginDTO.Password, user.Password))
            {
                return NotFound("Tài khoản hoặc mật khẩu không đúng!!");
            }
            var token = GenerateToken(loginDTO.UserName);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateNewUser(UserDTO userDTO)
        {
            
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(p => p.UserName == userDTO.UserName);
                    if (existingUser is not null)
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

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    return Ok("Thêm thành công");
                }
                catch (Exception)
                {
                    return StatusCode(500, $"Lỗi khi tạo phiếu nhập");

                }
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            var existingUser = await _context.Users
                                .FirstOrDefaultAsync(g => g.Id == id);
            if (existingUser is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, userDTO.Id));
            }
            string hashPassword = HashPassword(userDTO.Password);

            existingUser.UserName = userDTO.UserName;
            existingUser.Email = userDTO.Email;
            existingUser.Password = hashPassword;
            
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }

        

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

        //Generate Token
        private string GenerateToken(string Username)
        {
            var hashedKey = HashPassword(_config.GetSection("Jwt:Key").Value); // Mã hóa khóa bí mật
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(hashedKey));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Username),
            };

            var token = new JwtSecurityToken(
                _config.GetSection("Jwt:Issuer").Value,
                _config.GetSection("Jwt:Audience").Value,
                claim,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
