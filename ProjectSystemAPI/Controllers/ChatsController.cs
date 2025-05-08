using System;
using System.Collections;
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
    public class ChatsController : ControllerBase
    {
        private readonly ProjectSystemNewContext _context;

        public ChatsController(ProjectSystemNewContext context)
        {
            _context = context;
        }

        // GET: api/Chats
        [HttpGet("My/{idUser}")]
        public async Task<ActionResult<IEnumerable<ChatDTO>>> GetMyChats(int idUser)
        {
            var chats = _context.ChatUsers.AsNoTracking().
                Where(s => s.IdUser == idUser).
                Select(s => s.IdChatNavigation.Id).ToList();
            return Ok(_context.Chats.Include(s => s.ChatUsers).
                Include(s => s.ChatUsers).
                ThenInclude(s => s.IdUserNavigation).
                AsNoTracking().
                Where(s => chats.Contains(s.Id) && s.IsDeleted == false).Select(s => (ChatDTO)s).ToList());
            //var list = _context.Chats.Include(d => d.ChatUsers)
            //    .Where(s => s.ChatUsers.FirstOrDefault(u => u.Id == idUser) != null)
            //    .ToList();
            ////list.RemoveAll(s => _context.TaskForUsers.FirstOrDefault(u => u.IdTask == s.Id && u.IdUser == idUser) == null);
            //return Ok(list.Select(s => (ChatDTO)s));
        }

        // GET: api/Chats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatDTO>> GetChat(int id)
        {
            var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            return (ChatDTO)chat;
        }

        // PUT: api/Chats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChat(int id, ChatDTO chat)
        {
            if (id != chat.Id)
            {
                return BadRequest();
            }

            _context.Entry((Chat)chat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
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

        // POST: api/Chats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChatDTO>> PostChat(ChatDTO chat)
        {
            var result = (Chat)chat;
            _context.Chats.Add(result);
            await _context.SaveChangesAsync();
            chat.Id = result.Id;
            return CreatedAtAction("GetChat", new { id = result.Id }, chat);
        }

        [HttpGet("ChatMembers/{id}")]
        public async Task<ActionResult<List<UserDTO>>> ChatMembers(int id)
        {
            return Ok(_context.ChatUsers.Include(s => s.IdChatNavigation).AsNoTracking().Where(s => s.IdChat == id).Select(s => s.IdUserNavigation).ToList());
        }

        [HttpPost("AddNewMembers/{id}")]
        public async Task<ActionResult> AddNewMembers(int id, [FromBody] List<UserDTO> chatUsers)
        {
            var remove = _context.ChatUsers.Where(s => s.IdChat == id);
            _context.ChatUsers.RemoveRange(remove);

            ChatUser chatUser = new ChatUser();

            foreach (var member in chatUsers)
            {
                chatUser = new ChatUser
                {
                    IdChat = id,
                    IdUser = member.Id,
                    IdChatNavigation = _context.Chats.Find(id),
                    IdUserNavigation = _context.Users.Find(member.Id)
                };
                _context.ChatUsers.Add(chatUser);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("FindChat/{idUser}")]
        public async Task<ActionResult<List<ChatDTO>>> FindChat(int idUser, [FromBody] string find )
        {
            List<Chat> chatList = new List<Chat>();
            chatList.AddRange(_context.Chats.Where(s => s.Title.Contains(find)).AsNoTracking().ToList());
            chatList = chatList.Union(_context.ChatUsers.Include(s => s.IdUserNavigation).
                Where(s => s.IdUserNavigation.LastName.Contains(find) || s.IdUserNavigation.FirstName.Contains(find) || s.IdUserNavigation.Patronymic.Contains(find)).AsNoTracking().ToList().
                Select(s => _context.Chats.Find(s.IdChat))).ToList();

            chatList.RemoveAll(s => _context.ChatUsers.FirstOrDefault(u => u.IdChat == s.Id && u.IdUser == idUser) == null);
            return Ok(chatList.Select(s => (ChatDTO)s).Where(s => s.IsDeleted == false).Distinct());
        }
        // DELETE: api/Chats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }
    }
}
