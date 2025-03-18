using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSystemAPI.DB;

namespace ProjectSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskMsController : ControllerBase
    {
        private readonly ProjectSystemNewContext _context;

        public TaskMsController(ProjectSystemNewContext context)
        {
            _context = context;
        }

        // GET: api/TaskMs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskM>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        // GET: api/TaskMs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskM>> GetTaskM(int id)
        {
            var taskM = await _context.Tasks.FindAsync(id);

            if (taskM == null)
            {
                return NotFound();
            }

            return taskM;
        }

        // PUT: api/TaskMs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskM(int id, TaskM taskM)
        {
            if (id != taskM.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskM).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskMExists(id))
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

        // POST: api/TaskMs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskM>> PostTaskM(TaskM taskM)
        {
            _context.Tasks.Add(taskM);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskM", new { id = taskM.Id }, taskM);
        }

        // DELETE: api/TaskMs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskM(int id)
        {
            var taskM = await _context.Tasks.FindAsync(id);
            if (taskM == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskM);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskMExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
