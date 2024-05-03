using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using System.Security.AccessControl;


namespace AccountManagermnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceivedNoteController : BaseDataController
    {
        private readonly AccountDbContext _context;

        public GoodsReceivedNoteController(AccountDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<GoodsReceivedNote>> GetAllGRN(string? search, int offset, int limit)
        {
            var query = _context.GoodsReceivedNotes.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.GRNId.ToString().Contains(search));

            }
            var grnList = await query.OrderBy(x => x.GRNId)
                                     .Skip(offset)
                                     .Take(limit)
                                     .Select(g => new GoodsReceivedNote
                                     {
                                         GRNId = g.GRNId,
                                         DocumentDay = g.DocumentDay,
                                         DocumentNumber = g.DocumentNumber,
                                         Detail = g.Detail,
                                         PersonID = g.PersonID,
                                         GoodsReceivedNoteDetails = g.GoodsReceivedNoteDetails
                                         .Select(d => new GoodsReceivedNoteDetail
                                         {
                                             GRNDId = d.GRNDId,
                                             WarehousId = d.WarehousId,
                                             Quantity = d.Quantity,
                                             UnitPirce = d.UnitPirce,
                                             DebitAccount = d.DebitAccount,
                                             CreditAccount = d.CreditAccount,
                                             GRN_Id = d.GRN_Id,
                                             ProductId = d.ProductId,
                                             TotalPrice = d.TotalPrice,
                                         }).ToList(),

                                     })
                                     .ToListAsync();

            var pageResult = new PageResult<GoodsReceivedNote>(offset, limit, 0, 0, grnList);
            pageResult.Pos = offset;
            pageResult.total_count = 0;
            if (offset == 0)
            {
                pageResult.total_count = await query.CountAsync();
            }
            return Ok(pageResult);

            

            ////cách 1
            //var listGRN = await _context.GoodsReceivedNotes
            //            .Include(g => g.Person)
            //            .Include(g => g.GoodsReceivedNoteDetails)
            //            .ThenInclude(g => g.ProductCategorys)
            //            .ToListAsync();
            //return Ok(listGRN);

        }

        [HttpPost]
        public async Task<ActionResult<GoodsReceivedNote>> CreateNewGRN(GoodsReceivedNoteDTO goodsReceivedNoteDTO)
        {
            using (IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingGRN = await _context.GoodsReceivedNotes.FirstOrDefaultAsync(g => g.GRNId == goodsReceivedNoteDTO.GRNId);
                    if (existingGRN is not null)
                    {
                        await transaction.RollbackAsync();
                        return Conflict(ErrorConst.ID_IS_EXISTS);
                    }
                    
                    var newGRN = new GoodsReceivedNote
                    {
                        DocumentDay = goodsReceivedNoteDTO.DocumentDay,
                        DocumentNumber = goodsReceivedNoteDTO.DocumentNumber,
                        Detail = goodsReceivedNoteDTO.Detail,
                        PersonID = goodsReceivedNoteDTO.PersonID,
                    };
                    
                    _context.GoodsReceivedNotes.Add(newGRN);
                    await _context.SaveChangesAsync();           
                        foreach (var detailDTO in goodsReceivedNoteDTO.GoodsReceivedNoteDetails)
                        {
                            var creditAcc = detailDTO.CreditAccount;
                            var debitAcc = detailDTO.DebitAccount;

                            // Kiểm tra xem các tài khoản đã tồn tại trong bảng AccountCategory chưa
                            var existingCreditAcc = await _context.AccountCategorys.FindAsync(creditAcc);
                            if (existingCreditAcc == null)
                            {
                                return NotFound("Không tìm  thấy tài có này");
                            }
                            var existingDebitAcc = await _context.AccountCategorys.FindAsync(debitAcc);
                            if (existingCreditAcc == null)
                            {
                                return NotFound("Không tìm  thấy tài khoản nợ này");
                            }

                            var newGRNDetail = new GoodsReceivedNoteDetail
                                {
                                    WarehousId = detailDTO.WarehousId,
                                    Quantity = detailDTO.Quantity,
                                    UnitPirce = detailDTO.UnitPirce,
                                    DebitAccount = debitAcc,
                                    CreditAccount = creditAcc,
                                    GRN_Id = newGRN.GRNId,
                                    ProductId = detailDTO.ProductId,
                                    TotalPrice = (long) detailDTO.Quantity * detailDTO.UnitPirce,
                                };

                                // Thêm goodsReceivedNoteDetail vào cơ sở dữ liệu
                                _context.GoodsReceivedNoteDetails.Add(newGRNDetail);
                            }
                    
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return Ok("Thêm thành công");

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Lỗi khi tạo phiếu nhập");

                }

            }

        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<GoodsReceivedNote>> UpdateGRN(int id, [FromBody] GoodsReceivedNoteDTO goodsReceivedNoteDTO)
        {
            var existingGRN = await _context.GoodsReceivedNotes
                                .Include(d => d.GoodsReceivedNoteDetails)
                                .FirstOrDefaultAsync(g => g.GRNId == id);
            if (existingGRN is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, goodsReceivedNoteDTO.GRNId));
            }

            // Cập nhật các thuộc tính của GoodsReceivedNote
            existingGRN.DocumentDay = goodsReceivedNoteDTO.DocumentDay;
            existingGRN.DocumentNumber = goodsReceivedNoteDTO.DocumentNumber;
            existingGRN.Detail = goodsReceivedNoteDTO.Detail;
            existingGRN.PersonID = goodsReceivedNoteDTO.PersonID;

            //Xóa goodsReceivedNoteDetail hiện tại
            _context.GoodsReceivedNoteDetails.RemoveRange(existingGRN.GoodsReceivedNoteDetails);

            foreach (var detailDTO in goodsReceivedNoteDTO.GoodsReceivedNoteDetails)
            {
                //Thêm goodsReceivedNoteDetail mới
                existingGRN.GoodsReceivedNoteDetails.Add(new GoodsReceivedNoteDetail
                {
                    WarehousId = detailDTO.WarehousId,
                    Quantity = detailDTO.Quantity,
                    UnitPirce = detailDTO.UnitPirce,
                    DebitAccount = detailDTO.DebitAccount,
                    CreditAccount = detailDTO.CreditAccount,
                    GRN_Id = detailDTO.GRN_Id,
                    ProductId = detailDTO.ProductId,
                    TotalPrice = (long)detailDTO.Quantity * detailDTO.UnitPirce,
                }); ;
            }
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingGRN = await _context.GoodsReceivedNotes.FirstOrDefaultAsync(p => p.GRNId == id);
            if (existingGRN is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS));

            }
            _context.GoodsReceivedNotes.Remove(existingGRN);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }
    }
}
