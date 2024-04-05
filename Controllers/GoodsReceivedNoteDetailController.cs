 using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AccountManagermnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceivedNoteDetailController : ControllerBase
    {
        private readonly AccountDbContext _context;

        public GoodsReceivedNoteDetailController(AccountDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PageResult<GoodsReceivedNoteDetail>>> GetGRNdetail(string? search, int offset, int limit)
        {
            var query = _context.GoodsReceivedNoteDetails.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(gd => gd.GRNDId.ToString().Contains(search));
            }
            var GRNdetail = await query.OrderBy(gd => gd.GRNDId)
                                       .Skip(offset)
                                       .Take(limit)
                                       .Select( g => new GoodReceivedNoteDetailDTO
                                       {
                                          GRNDId = g.GRNDId,
                                          WarehousId = g.WarehousId,
                                          Quantity = g.Quantity,
                                          UnitPirce = g.UnitPirce,
                                          DebitAccount = g.DebitAccount,
                                          CreditAccount = g.CreditAccount,
                                          GRN_Id = g.GRN_Id,
                                          ProductId = g.ProductId
                                       })
                                       .ToListAsync();

            var pageResult = new PageResult<GoodReceivedNoteDetailDTO>(offset, limit, 0, 0, GRNdetail);
            pageResult.Pos = offset;
            pageResult.total_count = 0;
            if (offset == 0)
            {
                pageResult.total_count = await query.CountAsync();
            }

            
            //var listGRND = await _context.GoodsReceivedNoteDetails
            //    .Include(g => g.ProductCategorys)
            //    .Select(g => new
            //    {
            //        g.GRNDId,
            //        g.WarehousId,
            //        g.Quantity,
            //        g.UnitPirce,
            //        g.DebitAccount,
            //        g.CreditAccount,
            //        g.GRN_Id,
            //        g.ProductId,
      
            //    })
            //    .ToListAsync();
            //return Ok(listGRND);

            return Ok(pageResult);


        }
        [HttpPost]
        public async Task<ActionResult<GoodsReceivedNoteDetail>> CreateNewGRNdetail(GoodReceivedNoteDetailDTO goodReceivedNoteDetailDTO)
        {
            var existingGRNdetail = await _context.GoodsReceivedNoteDetails.FirstOrDefaultAsync(gd => gd.GRNDId == goodReceivedNoteDetailDTO.GRNDId);
            if (existingGRNdetail is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            var newGRNdetail = new GoodsReceivedNoteDetail
            {
                WarehousId = goodReceivedNoteDetailDTO.WarehousId,
                Quantity = goodReceivedNoteDetailDTO.Quantity,
                UnitPirce = goodReceivedNoteDetailDTO.UnitPirce,
                DebitAccount = goodReceivedNoteDetailDTO.DebitAccount,
                CreditAccount = goodReceivedNoteDetailDTO.CreditAccount,
                GRN_Id = goodReceivedNoteDetailDTO.GRN_Id,
                ProductId = goodReceivedNoteDetailDTO.ProductId,
            };
            _context.GoodsReceivedNoteDetails.Add(newGRNdetail);
            await _context.SaveChangesAsync();
            return Ok(newGRNdetail);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<GoodsReceivedNoteDetail>> UpdateGRNdetail(int id,GoodReceivedNoteDetailDTO goodReceivedNoteDetailDTO )
        {
            var existingGRNdetail = await _context.GoodsReceivedNoteDetails.FirstOrDefaultAsync(gd => gd.GRNDId == id);
            if (existingGRNdetail is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, goodReceivedNoteDetailDTO.GRNDId));
            }
            existingGRNdetail.GRNDId = goodReceivedNoteDetailDTO.GRNDId;
            existingGRNdetail.WarehousId = goodReceivedNoteDetailDTO.WarehousId;
            existingGRNdetail.Quantity = goodReceivedNoteDetailDTO.Quantity;
            existingGRNdetail.UnitPirce = goodReceivedNoteDetailDTO.UnitPirce;
            existingGRNdetail.DebitAccount = goodReceivedNoteDetailDTO.DebitAccount;
            existingGRNdetail.CreditAccount = goodReceivedNoteDetailDTO.CreditAccount;
            existingGRNdetail.GRN_Id = goodReceivedNoteDetailDTO.GRN_Id;
            existingGRNdetail.ProductId = goodReceivedNoteDetailDTO.ProductId;

            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingGRNdetail = await _context.GoodsReceivedNoteDetails.FirstOrDefaultAsync(gd => gd.GRNDId == id);
            if (existingGRNdetail is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS));

            }
            _context.GoodsReceivedNoteDetails.Remove(existingGRNdetail);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }
    }
}
