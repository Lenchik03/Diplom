using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChatServerDTO.DB;
using ChatServerDTO.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using Task = ChatServerDTO.DB.Task;

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


        [HttpGet("My/{idUser}")]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetMyTasks(int idUser)
        {
            var list = _context.TaskForUsers.Include(s => s.IdTaskNavigation).ThenInclude(s => s.IdCreatorNavigation).Include(s => s.IdTaskNavigation).ThenInclude(s => s.IdStatusNavigation).Include(s => s.IdTaskNavigation).ThenInclude(s => s.IdProjectNavigation).Include(s => s.IdTaskNavigation).ThenInclude(s => s.TaskForUsers).ThenInclude(s => s.IdUserNavigation).Include(s => s.IdTaskNavigation).ThenInclude(s => s.TaskForUsers).ThenInclude(s => s.IdStatusNavigation).AsNoTracking().Where(s => s.IdUser == idUser).Select(s => s.IdTaskNavigation).OrderByDescending(s => s.Id).ToList();
            return Ok(list.Select(s => (TaskDTO)s).OrderByDescending(s => s.Id));
            /*var list = _context.Tasks.Include(d=>d.TaskForUsers)
                .Where(s=>s.TaskForUsers.FirstOrDefault(u=>u.Id == idUser) != null)
                .ToList();
            //list.RemoveAll(s => _context.TaskForUsers.FirstOrDefault(u => u.IdTask == s.Id && u.IdUser == idUser) == null);
            return Ok(list.Select(s => (TaskDTO)s));*/
        }

        [HttpPost("AddNewExecutors/{id}")]
        public async Task<ActionResult> AddNewExecutors(int id, [FromBody] List<UserDTO> executors)
        {
            var remove = _context.TaskForUsers.Where(s => s.IdTask == id);
            _context.TaskForUsers.RemoveRange(remove);
            

            TaskForUser taskForUser = new TaskForUser();

            foreach (var ex in executors)
            {
                taskForUser = new TaskForUser
                {
                    IdTask = id,
                    IdUser = ex.Id,
                    IdTaskNavigation = _context.Tasks.Find(id),
                    IdUserNavigation = _context.Users.Find(ex.Id)
                };
                _context.TaskForUsers.Add(taskForUser);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Filter/{id}")]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> Search(int id)
        {
            var list = _context.Tasks.AsNoTracking().Where(s => s.IdStatus == id).ToList();
            return Ok(list.Select(s => (TaskDTO)s));
        }


        // GET: api/TaskMs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetTasks(int idProject)
        {
            var lists = await _context.Tasks.Where(s => s.IdProject == idProject).AsNoTracking().
                Include(s => s.IdProjectNavigation).
                Include(s => s.IdCreatorNavigation).
                Include(s => s.IdStatusNavigation).
                Include(s => s.TaskForUsers).Include("TaskForUsers.IdStatusNavigation").Include("TaskForUsers.IdUserNavigation").ToListAsync();
            return Ok(lists.Select(s => (TaskDTO)s).OrderByDescending(s => s.Id));

        }

        // GET: api/TaskMs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDTO>> GetTaskM(int id)
        {
            var taskM = await _context.Tasks.FindAsync(id);

            if (taskM == null)
            {
                return NotFound();
            }

            return (TaskDTO)taskM;
        }

        // PUT: api/TaskMs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskM(int id, TaskDTO taskM)
        {
            if (id != taskM.Id)
            {
                return BadRequest();
            }
            
            _context.Entry((Task)taskM).State = EntityState.Modified;

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
        public async Task<ActionResult<Task>> PostTaskM(TaskDTO taskM)
        {
            var task = (Task)taskM;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            taskM.Id = task.Id;
            return CreatedAtAction("GetTaskM", new { id = task.Id }, taskM);
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
