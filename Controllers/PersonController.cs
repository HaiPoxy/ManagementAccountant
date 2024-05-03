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
    public class PersonController : BaseDataController
    {
        private readonly  AccountDbContext _context;

        public PersonController(AccountDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<PageResult<Person>>> GetAllPerson(string? search, int offset, int limit)
        {
            var query = _context.Persons.AsQueryable();
            if (!string.IsNullOrEmpty(search) )
            {
                query = query.Where(p => p.PName.Contains(search) || p.Id.Contains(search));
            }
            var personList = await query.OrderBy(a=>a.Id)
                                .Skip(offset)
                                .Take(limit)
                                .ToListAsync();
            //var totalItems = await _context.Persons.CountAsync();
            //var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            //if(page < 1)
            //    page = 1;
            //else if(page > totalPages)
            //    page = totalPages;
            //var personList = await _context.Persons.OrderBy(b => b.Id)
            //                .Skip((page - 1) * pageSize)
            //                .Take(pageSize)
            //                .ToListAsync();
            //var pageResult = new PageResult<Person>(page, pageSize, totalItems, totalPages, personList);
            var pageResult = new PageResult<Person>(offset, limit, 0, 0, personList);
            pageResult.Pos = offset;
            pageResult.total_count = 0;
            if (offset == 0)
            {
                pageResult.total_count = await query.CountAsync();
            }
            return Ok(pageResult);
        }
        [HttpPost]
        public async Task<ActionResult<Person>> CreateNewPerson(PersonDTO personDTO)
        {
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Id == personDTO.Id); 
            if (existingPerson is not null) 
            {
                return Conflict(ErrorConst.ID_IS_EXISTS);
            }
            var newPerson = new Person
            {
                Id = personDTO.Id,
                PName = personDTO.PName,
                Address = personDTO.Address,
                PhoneNumber = personDTO.PhoneNumber,
                Email = personDTO.Email,
                TaxCode = personDTO.TaxCode
            };
            _context.Persons.Add(newPerson);
            await _context.SaveChangesAsync();
            return Ok(newPerson);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> UpdatePerson(string id, PersonDTO personDTO)
        {
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPerson is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS, personDTO.Id));
            }
            existingPerson.PName = personDTO.PName;
            existingPerson.Address = personDTO.Address;
            existingPerson.PhoneNumber = personDTO.PhoneNumber;
            existingPerson.Email = personDTO.Email;
            existingPerson.TaxCode = personDTO.TaxCode;
            
            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPerson is null)
            {
                return NotFound(string.Format(ErrorConst.ID_IS_NOT_EXISTS));

            }
            _context.Persons.Remove(existingPerson);
            await _context.SaveChangesAsync();
            return Ok("xóa thành công");
        }
 
    }
}
