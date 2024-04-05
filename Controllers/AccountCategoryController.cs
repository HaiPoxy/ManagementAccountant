using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

namespace AccountManagermnet.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCategoryController : ControllerBase
    {
        private readonly AccountDbContext _context;
        
        private List<AccountCategory> accountTable = new List<AccountCategory>();
        public AccountCategoryController(AccountDbContext context)
        {
            _context = context; 
            
        }

        //[HttpGet]
        //public async Task<ActionResult<List<AccountCategory>>> GetAccount()
        //{
        //    return Ok(await _context.AccountCategorys.OrderBy(account => account.Id).ToListAsync());
        //}

        [HttpGet("accounts")]
        public async Task<ActionResult<List<AccountCategory>>> GetAccountTree()
        {
            var accountList = new List<AccountCategory>();

            //Lấy ra danh sách các nút cha, những tài khoản có parentID rỗng thì sẽ là nút cha
            var parentNodes = await _context.AccountCategorys.Where(a => string.IsNullOrEmpty(a.ParentId)).ToListAsync();

            //Hiển thị các tài khoản con theo các tài khoản cha
            foreach (var allParentNode in parentNodes)
            {
                GetAccountBinaryTree(accountList, allParentNode);
            }
            return Ok(accountList);
        }

        //[HttpGet]
        //public async Task<ActionResult<PageResult<AccountCategory>>> GetAccountTree(int offset, int limit)
        //{

        //    var accountList = await _context.AccountCategorys
        //                    .OrderBy(b => b.Id)
        //                    .Skip(offset)
        //                    .Take(limit)
        //                    .ToListAsync();

        //    var pageResult = new PageResult<AccountCategory>(offset, limit, 0, 0, accountList);
        //    pageResult.Pos = offset;
        //    pageResult.total_count = 0;
        //    if (offset == 0)
        //    {
        //        pageResult.total_count = await _context.AccountCategorys.CountAsync();
        //    }
        //    return Ok(pageResult);

        //}
        [HttpGet]
        public async Task<ActionResult<List<AccountCategory>>> GetAccount(string? search, int offset, int limit)
        {
            
            {

                var query = _context.AccountCategorys.AsQueryable();
                // Kiểm tra nếu có name yêu cầu tìm kiếm theo tên
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(a => (a.Name.Contains(search) || a.Id.Contains(search)));
                }
 
                var accountList = await query.OrderBy(a => a.Id)
                                  .Skip(offset)
                                  .Take(limit)
                                  .ToListAsync(); 
               
                // hiển thị kết quả phân trang 
                var pageResult = new PageResult<AccountCategory>(offset, limit, 0, 0, accountList);
                pageResult.Pos = offset;
                pageResult.total_count = 0;
                if (offset == 0)
                {
                    pageResult.total_count = await query.CountAsync();
                }

                return Ok(pageResult);
            }

        }


        //[HttpGet]
        //public async Task<ActionResult<List<AccountCategory>>> GetAccountByname(string? name, string id, int offset, int limit)
        //{

        //    {

        //        if (string.IsNullOrEmpty(name))
        //        {
        //            // Trả về toàn bộ danh sách tài khoản nếu không có tên được nhập
        //            //var allAccounts = await _context.AccountCategorys.ToListAsync();
        //            //return allAccounts;
        //            var accountList = await _context.AccountCategorys
        //                    .OrderBy(b => b.Id)
        //                    .Skip(offset)
        //                    .Take(limit)
        //                    .ToListAsync();

        //            var pageResult = new PageResult<AccountCategory>(offset, limit, 0, 0, accountList);
        //            pageResult.Pos = offset;
        //            pageResult.total_count = 0;
        //            if (offset == 0)
        //            {
        //                pageResult.total_count = await _context.AccountCategorys.CountAsync();
        //            }
        //            return Ok(pageResult);
        //        }

        //        // Tìm kiếm tài khoản theo tên
        //        var accounts = await _context.AccountCategorys.Where(a => a.Name.Contains(name))
        //                                    .OrderBy(b => b.Id)
        //                                    .Skip(offset)
        //                                    .Take(limit)
        //                                    .ToListAsync();

        //        var pageResultByName = new PageResult<AccountCategory>(offset, limit, 0, 0, accounts);
        //        pageResultByName.Pos = offset;
        //        pageResultByName.total_count = 0;
        //        if (offset == 0)
        //        {
        //            pageResultByName.total_count = await _context.AccountCategorys.CountAsync();
        //        }

        //        if (accounts.Count == 0)
        //        {
        //            return NotFound("Không tìm thấy tài khoản nào.");
        //        }
        //        return accounts;


        //    }

        //}

        //[HttpGet("{parentId}/account")]
        //public async Task<ActionResult<List<AccountCategory>>> accountChild(string parentId)
        //{
        //    if(parentId.Length == 3)
        //    {
        //        var accountChild = await _context.AccountCategorys
        //                                .Where(account => account.Id.StartsWith(parentId) && account.Id.Length > parentId.Length)
        //                                .OrderBy(account => account.Id)
        //                                .ToListAsync();
        //        return Ok(accountChild);
        //    }
        //    else
        //    {
        //        return BadRequest("parentId phải = 3");
        //    }
        //}
        [HttpGet("childaccounts/{parentId}")]
        public async Task<ActionResult<List<AccountCategory>>> GetChildAccounts(string parentId)
        {
            try
            {
                // Lấy danh sách các tài khoản con có ParentId trùng với parentId được nhập vào
                var childAccounts = await _context.AccountCategorys.Where(account => account.ParentId == parentId).ToListAsync();

                if (childAccounts == null || childAccounts.Count == 0)
                {
                    return NotFound("Không tìm thấy tài khoản con cho parentId đã nhập.");
                }

                return Ok(childAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách tài khoản con: {ex.Message}");
            }
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<AccountCategory>> GetAccountById(string id)
        {
            var account = await _context.AccountCategorys.FindAsync(id);
            if (account == null)
            {
                return NotFound(ErrorConst.ID_IS_NOT_EXISTS);
            }
            return account;
        }        /* */


        [HttpGet("ParentId")]
        public async Task<ActionResult<AccountCategory>> GetAllParentId()
        {
             var ParentId = await _context.AccountCategorys.Select(a => a.ParentId).Distinct().ToListAsync();
             return Ok(ParentId);
        }

        [HttpPost]
        public async Task<ActionResult<AccountCategory>> Create(AccountCategoryDTO accountCategoryDTO)
        {
            // Kiểm tra xem Id có đủ 3 ký tự không
            if (accountCategoryDTO.Id.Length < 3)
            {
                return BadRequest(ErrorConst.ID_LENGHT_GREATER_THAN_3);
            }
            // kiểm tra xem tài khoản đã tồn tại chưa
            var existingAccount = await _context.AccountCategorys.FirstOrDefaultAsync(account => account.Id == accountCategoryDTO.Id);
            if(existingAccount is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            //kiểm tra Id và ParentId có trùng nhau không 
            if(accountCategoryDTO.Id == accountCategoryDTO.ParentId)
            {
                return BadRequest(ErrorConst.ID_AND_PARENT_ID_IS_NOT_OVERLAP);
            }
            //

            //kiểm tra ParentID có tồn tại không 
            var isParentIdExist = await this.IsExistParentId2(accountCategoryDTO.ParentId);
            if (!isParentIdExist && !string.IsNullOrEmpty(accountCategoryDTO.ParentId))
            {
                return NotFound(string.Format(ErrorConst.PARENT_ID_NOT_FOUND,accountCategoryDTO.ParentId));
            }

            var childAccount = new AccountCategory
            {
                Id = accountCategoryDTO.Id,
                Name = accountCategoryDTO.Name,
                BankAccount = accountCategoryDTO.BankAccount,
                BankName = accountCategoryDTO.BankName,
                ParentId = accountCategoryDTO.ParentId
            };
            ////nếu parentId rỗng thì Id sẽ là parentId mới
            //if (string.IsNullOrWhiteSpace(accountCategoryDTO.ParentId))
            //{
            //    childAccount.ParentId = accountCategoryDTO.Id;
            //}
            // Thêm tài khoản con vào cơ sở dữ liệu
            _context.AccountCategorys.Add(childAccount);
            await _context.SaveChangesAsync();

            return Ok(childAccount);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, AccountCategoryDTO accountCategoryDTO)
        {

            //if (await _context.AccountCategorys.AnyAsync(account => account.Id == accountCategory.Id))
            //{
            //    return BadRequest("Id đã tồn tại");
            //}
 
            //kiểm tra id tồn tại không
            var existingAccount = await _context.AccountCategorys.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAccount == null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, accountCategoryDTO.Id));
            }
            //kiểm tra ParentID có tồn tại không 
            var isParentIdExist = await this.IsExistParentId(accountCategoryDTO.ParentId);
            if (!isParentIdExist && !string.IsNullOrEmpty(accountCategoryDTO.ParentId))
            {
                return NotFound(string.Format(ErrorConst.PARENT_ID_NOT_FOUND, accountCategoryDTO.ParentId));
            }
            //tồn tại các tài khoản con 
            var childAccount = await _context.AccountCategorys.AnyAsync(a => a.ParentId == id);
            if (childAccount)
            {
                return BadRequest("không thể sửa nút cha khi có con");
            }
            existingAccount.Name = accountCategoryDTO.Name;
            existingAccount.BankAccount = accountCategoryDTO.BankAccount;
            existingAccount.BankName = accountCategoryDTO.BankName;
            existingAccount.ParentId = accountCategoryDTO.ParentId;
            //_context.Entry(accountCategoryDTO).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }
        

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, AccountCategoryDTO accountCategoryDTO)
        {
            //tìm kiếm id tài khoản cần xóa
            //var account = _context.AccountCategorys.Find(id);
            //if (account == null)
            //{
            //    return NotFound(ErrorConst.ID_IS_NOT_EXISTS);
            //}
            var existingAccount = await _context.AccountCategorys.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAccount == null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_FOUND, accountCategoryDTO.Id));
            }

            //tồn tại các tài khoản con 
            var childAccount = await _context.AccountCategorys.AnyAsync(a => a.ParentId == id);
            if (childAccount)
            {
                return BadRequest("không thể xóa nút cha khi có con");
            }
            _context.AccountCategorys.Remove(existingAccount);
            await _context.SaveChangesAsync();
            return Ok("Xóa thành công");
            
        }

        #region Privates
        private async Task<bool> IsExistParentId2(string parentId)
        {
            // nếu parentId trống thì sẽ trả về 
            if (string.IsNullOrWhiteSpace(parentId)) return false;

            //lấy ra tài khoản account có parentid = parentID
            var isExisted = await _context.AccountCategorys.AnyAsync(p => string.Equals(p.Id,parentId));
            //var isExisted = await _context.AccountCategorys.AnyAsync(p => p.id == parentId);

            return isExisted;
        }
        private async Task<bool> IsExistParentId(string parentId)
        {
            if (string.IsNullOrWhiteSpace(parentId)) return false;
            var isExisted = await _context.AccountCategorys.AnyAsync(p => string.Equals(p.ParentId, parentId));
            return isExisted;
        }
        private async Task<bool> IsExistsId(string Id)
        {
            if(string.IsNullOrWhiteSpace(Id)) return false;
            var isExisted = await _context.AccountCategorys.AnyAsync(a =>  string.Equals(a.Id, Id));
            return isExisted;
        }
        private async Task<bool> IsExistName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var isExisted = await (_context.AccountCategorys.AnyAsync(n => string.Equals(n.Name, name)));
            return isExisted;
        }
        private void GetAccountBinaryTree(List<AccountCategory> accountTree, AccountCategory parentNode)
        {
            //những tài khoản mà nút cha không tồn tại sẽ không in ra
            if (parentNode == null)
            {
                return;
            }
            //thêm parentNode vào danh sách
            accountTree.Add(parentNode);

            //lấy danh sách các con của parent Node từ cơ sở dữ liệu
            var children = _context.AccountCategorys.Where(a => a.ParentId == parentNode.Id).ToList();

            //Thêm các tài khoản con vào theo parentNode đã thêm vào danh sách 
            foreach (var child in children.OrderBy(c => c.Id))
            {
                GetAccountBinaryTree(accountTree, child);
            }

        }
        #endregion
    }
}
