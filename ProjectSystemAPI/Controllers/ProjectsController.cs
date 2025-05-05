using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerDTO.DB;
using ChatServerDTO.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;

namespace ProjectSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectSystemNewContext _context;

        public ProjectsController(ProjectSystemNewContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            var projects = await _context.Projects.Include(s => s.Tasks).Include(s => s.IdCreatorNavigation).Where(r => r.IsDeleted == false).AsNoTracking().OrderByDescending(s => s.StartDate).ToListAsync();
            return Ok(projects.Select(s => (ProjectDTO)s));
        }

        [HttpGet("MyProject/{idUser}")]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetMyProjects(int idUser)
        {
            var h = _context.TaskForUsers.Include(s => s.IdTaskNavigation).ThenInclude(s => s.IdProjectNavigation).Include("IdTaskNavigation.IdProjectNavigation.IdCreatorNavigation").
                Where(s => s.IdUser == idUser && s.IdTaskNavigation.IdProjectNavigation.IsDeleted == false).
                AsNoTracking();
            return Ok(h.Select(s => s.IdTaskNavigation).
                    Select(s => s.IdProjectNavigation).
                    Distinct().
                    OrderByDescending(s => s.StartDate).
                    Select(s=>(ProjectDTO)s).
                    ToList());
            /*var list = _context.Tasks.Include(d => d.TaskForUsers)
                .Where(s => s.TaskForUsers.FirstOrDefault(u => u.Id == idUser) != null)
                .Select(s => s.IdProject).Distinct().ToList();
            var result = _context.Projects.Where(s => list.Contains(s.Id)).ToList();
            //list.RemoveAll(s => _context.TaskForUsers.FirstOrDefault(u => u.IdTask == s.Id && u.IdUser == idUser) == null);
            return Ok(result);*/
        }

        [HttpGet("GetMyProjects/{idUser}")]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetMyProjects1(int idUser)
        {
            return Ok(_context.Projects.Where(s => s.IdCreator == idUser && s.IsDeleted == false).OrderByDescending(s => s.StartDate).Select(s => (ProjectDTO)s));
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return (ProjectDTO)project;
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, ProjectDTO project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry((Project)project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> PostProject(ProjectDTO project)
        {
            project.StartDate = DateTime.Now;
            _context.Projects.Add((Project)project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
