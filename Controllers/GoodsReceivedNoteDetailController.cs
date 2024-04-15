 using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Schema;

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
            var grnDetail = await query.OrderBy(gd => gd.GRNDId)
                                       .Skip(offset)
                                       .Take(limit)
                                       .Select( g => new GoodsReceivedNoteDetail
                                       {
                                          GRNDId = g.GRNDId,
                                          WarehousId = g.WarehousId,
                                          Quantity = g.Quantity,
                                          UnitPirce = g.UnitPirce,
                                          DebitAccount = g.DebitAccount,
                                          CreditAccount = g.CreditAccount,
                                          GRN_Id = g.GRN_Id,
                                          ProductId = g.ProductId,
                                          TotalPrice = g.TotalPrice,
                                       })
                                       .ToListAsync();

            var pageResult = new PageResult<GoodsReceivedNoteDetail>(offset, limit, 0, 0, grnDetail);
            pageResult.Pos = offset;
            pageResult.total_count = 0;
            if (offset == 0)
            {
                pageResult.total_count = await query.CountAsync();
            }


            return Ok(pageResult);


        }
        //private double CalculateTotal(int quantity, int unitPrice)
        //{
        //    return (double) quantity * unitPrice;
        //}

        [HttpGet("details/{grnId}")]
        public async Task<ActionResult<List<GoodsReceivedNoteDetail>>> GetChildAccounts(int grnId)
        {
            try
            {

                // Lấy danh sách các tài khoản con có ParentId trùng với parentId được nhập vào
                var details = await _context.GoodsReceivedNoteDetails.Where(d => d.GRN_Id == grnId).ToListAsync();
       
                {
                    return NotFound("Không tìm thấy Chi Tiết Phiếu nào từ Mã phiếu đã nhập.");
                }

                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách Chi Tiết Phiếu: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<GoodsReceivedNoteDetail>> CreateNewGRNdetail(GoodReceivedNoteDetailDTO goodReceivedNoteDetailDTO)
        {
            var existingGRNdetail = await _context.GoodsReceivedNoteDetails.FirstOrDefaultAsync(gd => gd.GRNDId == goodReceivedNoteDetailDTO.GRNDId);
            if (existingGRNdetail is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            var debitAcc = goodReceivedNoteDetailDTO.DebitAccount;
            var creditAcc = goodReceivedNoteDetailDTO.CreditAccount;
            if(string.IsNullOrEmpty(creditAcc) || string.IsNullOrEmpty(debitAcc))
            {
                return BadRequest("Không được để trống thông tin tài khoản");
            }
            var existingDebitAcc = await _context.AccountCategorys.FindAsync(debitAcc);
            if (existingDebitAcc == null)
            {
                return NotFound("Tài khoản nợ này không tồn tại");
            }
            var existingCreditAcc = await _context.AccountCategorys.FindAsync(creditAcc);
            if (existingCreditAcc == null)
            {
                return NotFound("Tài khoản có này không tồn tại");
            }
            var newGRNdetail = new GoodsReceivedNoteDetail

            {
                WarehousId = goodReceivedNoteDetailDTO.WarehousId,
                Quantity = goodReceivedNoteDetailDTO.Quantity,      
                UnitPirce = goodReceivedNoteDetailDTO.UnitPirce,
                DebitAccount = debitAcc,
                CreditAccount = creditAcc,
                GRN_Id = goodReceivedNoteDetailDTO.GRN_Id,
                ProductId = goodReceivedNoteDetailDTO.ProductId,
                TotalPrice = goodReceivedNoteDetailDTO.Quantity * goodReceivedNoteDetailDTO.UnitPirce,
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
            existingGRNdetail.WarehousId = goodReceivedNoteDetailDTO.WarehousId;
            existingGRNdetail.Quantity = goodReceivedNoteDetailDTO.Quantity;
            existingGRNdetail.UnitPirce = goodReceivedNoteDetailDTO.UnitPirce;
            existingGRNdetail.DebitAccount = goodReceivedNoteDetailDTO.DebitAccount;
            existingGRNdetail.CreditAccount = goodReceivedNoteDetailDTO.CreditAccount;
            existingGRNdetail.GRN_Id = goodReceivedNoteDetailDTO.GRN_Id;
            existingGRNdetail.ProductId = goodReceivedNoteDetailDTO.ProductId;
            existingGRNdetail.TotalPrice = goodReceivedNoteDetailDTO.Quantity * goodReceivedNoteDetailDTO.UnitPirce;
            
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
