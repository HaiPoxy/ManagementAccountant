using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AccountManagermnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly AccountDbContext _context;

        public ProductCategoryController(AccountDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<ProductCategory>> GetAllProduct(string? search, int offset, int limit)
        {
            var query = _context.ProductCategories.AsQueryable();
            if(!string.IsNullOrEmpty(search))
            {
                query = query.Where(Pr=> Pr.Name.Contains(search) || Pr.Id.Contains(search));
            }

            var productlist = await query.OrderBy(pr => pr.Id)
                                              .Skip(offset)
                                              .Take(limit)
                                              .Select(p => new ProductCategoryDTO
                                              {
                                                  Name = p.Name, 
                                                  Id = p.Id, 
                                                  Unit = p.Unit, 
                                                  RevenueAcc = p.RevenueAcc, 
                                                  GOGSAcc = p.GOGSAcc,
                                                  GoodsAcc = p.GoodsAcc,
                                              })
         
                                              .ToListAsync();

            var pageResult = new PageResult<ProductCategoryDTO>(offset, limit, 0, 0, productlist);
            pageResult.Pos = offset;
            pageResult.total_count = 0;
            if (offset == 0)
            {
                pageResult.total_count = await query.CountAsync();
            }
            return Ok(pageResult);
        }

        [HttpPost]
        public async Task<ActionResult<ProductCategory>> CreateNewProduct(ProductCategoryDTO productCategoryDTO)
        {

            // kiểm tra xem tài khoản đã tồn tại chưa
            var existingProduct = await _context.ProductCategories.FirstOrDefaultAsync(product => product.Id == productCategoryDTO.Id);
            if (existingProduct is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            var revenueAcc = productCategoryDTO.RevenueAcc;
            var goodsAcc = productCategoryDTO.GoodsAcc;
            var gogsAcc = productCategoryDTO.GOGSAcc;

            // Kiểm tra xem các thông tin tài khoản cần thiết đã được cung cấp hay chưa
            if (string.IsNullOrEmpty(revenueAcc) || string.IsNullOrEmpty(goodsAcc) || string.IsNullOrEmpty(gogsAcc))
            {
                return BadRequest("Không được để trống thông tin tài khoản");
            }

            // Kiểm tra xem các tài khoản đã tồn tại trong bảng AccountCategory chưa
            var existingRevenueAcc = await _context.AccountCategorys.FindAsync(revenueAcc);
            if(existingRevenueAcc == null)
            {
                return NotFound("Không tìm  thấy tài khoản doanh thu này");
            }
            var existingGoodsAcc = await _context.AccountCategorys.FindAsync(goodsAcc);
            if(existingGoodsAcc == null)
            {
                return NotFound("Không tìm thấy tài khoản hàng hóa này");
            }
            var existingGogsAcc = await _context.AccountCategorys.FindAsync(gogsAcc);
            if(existingGogsAcc == null)
            {
                return NotFound("Không tìm thấy tài khoản giá vốn này");
            }

            if (string.Equals(revenueAcc,goodsAcc) || string.Equals(revenueAcc,gogsAcc) || string.Equals(goodsAcc, gogsAcc))
            {
                return BadRequest("Mỗi tài khoản chỉ có thể thuộc một trong ba loại tài khoản: Revenue, Goods, hoặc GOGS");
            }

            // Tạo mới Product với thông tin đã cung cấp
            var newProduct = new ProductCategory
            {
                Id = productCategoryDTO.Id,
                Name = productCategoryDTO.Name,
                Unit = productCategoryDTO.Unit,
                RevenueAcc = revenueAcc,
                GoodsAcc = goodsAcc,
                GOGSAcc = gogsAcc
            };

            _context.ProductCategories.Add(newProduct);
            await _context.SaveChangesAsync();

            return Ok(newProduct);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductCategory>> UpdateProduct(string id, ProductCategoryDTO productCategoryDTO)
        {
            var existingProduct = await _context.ProductCategories.FirstOrDefaultAsync(x => x.Id == id);
            if(existingProduct == null)
            {
                return BadRequest(string.Format(ErrorConst.ID_IS_NOT_EXISTS, productCategoryDTO.Id));
            }
            var revenueAcc = productCategoryDTO.RevenueAcc;
            var goodsAcc = productCategoryDTO.GoodsAcc;
            var gogsAcc = productCategoryDTO.GOGSAcc;

            // Kiểm tra xem các thông tin tài khoản cần thiết đã được cung cấp hay chưa
            if (string.IsNullOrEmpty(revenueAcc) || string.IsNullOrEmpty(goodsAcc) || string.IsNullOrEmpty(gogsAcc))
            {
                return BadRequest("Không được để trống thông tin tài khoản");
            }

            // Kiểm tra xem các tài khoản đã tồn tại trong bảng AccountCategory chưa
            var existingRevenueAcc = await _context.AccountCategorys.FindAsync(revenueAcc);
            if(existingRevenueAcc == null)
            {
                return NotFound("Tài khoản doanh thu này không tồn tại");
            }
            var existingGoodsAcc = await _context.AccountCategorys.FindAsync(goodsAcc);
            if(existingGoodsAcc == null)
            {
                return NotFound("Tài khoản hàng hóa này không tồn tại");
            }
            var existingGogsAcc = await _context.AccountCategorys.FindAsync(gogsAcc);
            if (existingGogsAcc == null)
            {
                return NotFound("Tài khoản giá vốn này không tồn tại");
            }

            if (string.Equals(revenueAcc,goodsAcc) || string.Equals(revenueAcc, gogsAcc) || string.Equals( goodsAcc,gogsAcc))
            {
                return BadRequest("Mỗi tài khoản chỉ có thể thuộc một trong ba loại tài khoản: Revenue, Goods, hoặc GOGS");
            }
            existingProduct.Name = productCategoryDTO.Name;
            existingProduct.Unit = productCategoryDTO.Unit;
            existingProduct.RevenueAcc = revenueAcc;
            existingProduct.GoodsAcc = goodsAcc;
            existingProduct.GOGSAcc = gogsAcc;
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingProduct = await _context.ProductCategories.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS));

            }
            _context.ProductCategories.Remove(existingProduct);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }

    }
}
