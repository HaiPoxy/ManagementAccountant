using AccountManagermnet.Constants;
using AccountManagermnet.Data;
using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace AccountManagermnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceivedNoteController : ControllerBase
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
                query = query.Where(g => g.GRNId.Contains(search));
            }
            var grnList = await query.OrderBy(x => x.GRNId)
                                     .Skip(offset)
                                     .Take(limit)
                                     .Select(g => new GoodsReceivedNoteDTO
                                     {
                                         GRNId = g.GRNId,
                                         DocumentDay = g.DocumentDay,
                                         DocumentNumber = g.DocumentNumber,
                                         Detail = g.Detail,
                                         PersonID = g.PersonID,
                                         GoodReceivedNoteDetails = g.GoodsReceivedNoteDetails
                                                .Select(d => d.GRNDId)
                                                .ToList()
                                         
                                     })
                                     .ToListAsync();

            var pageResult = new PageResult<GoodsReceivedNoteDTO>(offset, limit, 0, 0, grnList);
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
        public async Task<ActionResult<GoodsReceivedNote>> CreateNewGRN (GoodsReceivedNoteDTO goodsReceivedNoteDTO)
        {
            var existingGRN = await _context.GoodsReceivedNotes.FirstOrDefaultAsync(g => g.GRNId == goodsReceivedNoteDTO.GRNId);
            if (existingGRN is not null)
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            var newGRN = new GoodsReceivedNote
            {
                GRNId = goodsReceivedNoteDTO.GRNId,
                DocumentDay = goodsReceivedNoteDTO.DocumentDay,
                DocumentNumber = goodsReceivedNoteDTO.DocumentNumber,
                Detail = goodsReceivedNoteDTO.Detail,
                PersonID = goodsReceivedNoteDTO.PersonID,
            };
            _context.GoodsReceivedNotes.Add(newGRN);
            await _context.SaveChangesAsync();
            return Ok(newGRN);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<GoodsReceivedNote>> UpdateGRN(string id, GoodsReceivedNoteDTO goodsReceivedNoteDTO)
        {
            var existingGRN = await _context.GoodsReceivedNotes.FirstOrDefaultAsync(g => g.GRNId == id);
            if (existingGRN is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, goodsReceivedNoteDTO.GRNId));
            }
            existingGRN.DocumentDay = goodsReceivedNoteDTO.DocumentDay;
            existingGRN.DocumentNumber = goodsReceivedNoteDTO.DocumentNumber;
            existingGRN.Detail = goodsReceivedNoteDTO.Detail;
            existingGRN.PersonID = goodsReceivedNoteDTO.PersonID;

            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(string id)
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
