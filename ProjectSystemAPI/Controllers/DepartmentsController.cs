using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerDTO.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;

namespace ProjectSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ProjectSystemNewContext _context;

        public DepartmentsController(ProjectSystemNewContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetDepartments()
        {
            var deps = await _context.Departments.Include(s => s.Users).Include(s=>s.IdDirectorNavigation).Where(s => s.IsDeleted == false).ToListAsync();
            return Ok( deps.Select(s => (DepartmentDTO)s));
        }

        // GET: api/Departments/5
        /*[HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }*/

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDTO>> GetDepartment(int id)
        {
            var department = await _context.Departments.Include(s=>s.Users).
                Include(s=>s.IdDirectorNavigation).
                Include(s=>s.InverseIdMainDepNavigation).FirstOrDefaultAsync(s=>s.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return (DepartmentDTO)department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutDepartment(int id, Department department)
        //{
        //    if (id != department.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(department).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DepartmentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, DepartmentDTO department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            _context.Entry((Department)department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DepartmentDTO>> PostDepartment(DepartmentDTO department)
        {
           // department.IdMainDepNavigation = null;
            _context.Departments.Add((Department)department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.Id }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
               
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
