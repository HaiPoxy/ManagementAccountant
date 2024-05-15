using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AccountManagermnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AccountDbContext _context;

        public RoleController(AccountDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetRole()
        {
            var roles = await _context.Roles.OrderBy(r => r.Id)
                                            .Include(r => r.UserRoles)
                                            .ToListAsync();
            return Ok(roles);
        }
        [HttpPost]
        public async Task<ActionResult<Role>> CreateNewRole(RoleDTO roleDTO)
        {
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleDTO.Id);
            if (existingRole is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            var newRole = new Role
            {
                Id = roleDTO.Id,
                RoleName = roleDTO.RoleName,
            };
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();
                foreach (var userrole in roleDTO.UserRoles)
                {
                    var newUserRoles = new UserRole
                    {
                        UserId = userrole.UserId,
                        RoleId = userrole.RoleId,
                    };
                }
            await _context.SaveChangesAsync();
            return Ok(newRole);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> UpdateRole(string id, RoleDTO roleDTO)
        {
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleDTO.Id);
            if (existingRole is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            existingRole.RoleName = roleDTO.RoleName;
            _context.UserRoles.RemoveRange(existingRole.UserRoles);
            foreach (var userrole in roleDTO.UserRoles)
            {
                existingRole.UserRoles.Add(new UserRole 
                {
                    UserId = userrole.UserId,
                    RoleId = userrole.RoleId,
                });
            }
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id.Equals(id));
            if (existingRole is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS));

            }
            _context.Roles.Remove(existingRole);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }

    }
}
