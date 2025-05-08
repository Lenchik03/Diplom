using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatServerDTO.DB;
using ChatServerDTO.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;

namespace ProjectSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly ProjectSystemNewContext dbContext;
        public UsersController(ProjectSystemNewContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public class ResponseTokenAndRole
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public UserDTO User { get; set; }
        }

        public class AuthOptions
        {
            public const string ISSUER = "MyAuthServer"; // издатель токена
            public const string AUDIENCE = "MyAuthClient"; // потребитель токена
            const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
            public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }

        //[Authorize(Roles = "Администратор, Клиент")]
        [HttpGet("ActiveUser")]
        public ActionResult<ResponseTokenAndRole> ExamClientData(string login, string password)
        {
            var examUser = dbContext.Users.Include(s => s.IdRoleNavigation).Include(s=>s.IdDepartmentNavigation).ThenInclude(s => s.InverseIdMainDepNavigation).FirstOrDefault(s => s.Email == login);
            if (examUser == null)
            {
                return NotFound("Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные");
            }
            else
            {
                if (examUser.Password != Md5.HashPassword(password))
                {
                    

                    return NotFound("Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные");
                }
                else
                {

                    var result = examUser;
                    dbContext.SaveChanges();

                    if (examUser is null)
                        return Unauthorized();

                    var role = result.IdRoleNavigation.Title;
                    int id = examUser.Id;

                    // Создаём полезную нагрузку для токена
                    var claims = new List<Claim> {
                //Кладём Id (если нужно)
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                //Кладём роль
                new Claim(ClaimTypes.Role, role)
            };

                    // создаем JWT-токен
                    var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: AuthOptions.AUDIENCE,
                            //кладём полезную нагрузку
                            claims: claims,
                            //устанавливаем время жизни токена 2 минуты
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(20)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                    string token = new JwtSecurityTokenHandler().WriteToken(jwt);

                    return Ok(new ResponseTokenAndRole
                    {
                        Token = token,
                        Role = role,
                        User = (UserDTO)result
                    });
                    //return Ok(result);
                }
            }
        }

        [HttpGet("GetExecutorsByTask/{taskId}")]
        public ActionResult<List<UserDTO>> GetExecutorsByTask(int taskId)
        {
            var list = dbContext.TaskForUsers.Include(s => s.IdTaskNavigation).
                Where(s => s.IdTask == taskId).AsNoTracking().Select(s => s.IdUserNavigation).Distinct().ToList();
            return Ok(list.Select(s => (UserDTO)s));

            //var result = dbContext.TaskForUsers.Include(s => s.IdTaskNavigation).Where(s => s.IdTask == taskId);
            //return Ok(result);
        }

        [HttpPost("GetExecutorsForTask")]
        public ActionResult<List<UserDTO>> GetExecutorsForTask(int userId, [FromBody] string search)
        {

            var exs = new ObservableCollection<User>();
            var user = dbContext.Users.FirstOrDefault(s => s.Id == userId);
            var dep = dbContext.Departments.FirstOrDefault(s => s.Id == user.IdDepartment);
            var deps = dbContext.Departments.Where(s => s.IdMainDep == dep.Id);
            var users = dbContext.Users.ToList();
            exs.Add(user);
            if (dep.IdMainDep == null)
            {
                foreach (var d in deps)
                {
                    foreach (var usr in users.Where(s => s.IdDepartment == d.Id))
                    {
                        exs.Add(usr);
                    }

                }
            }
            else
            {
                foreach (var usr in dbContext.Users.Where(s => s.IdDepartment == dep.Id))
                {
                    exs.Add(usr);
                }




            }
            var rezult = new ObservableCollection<User>(exs.Where(s => string.IsNullOrEmpty(search) ||
                        (s.FirstName.Contains(search) ||
                        s.LastName.Contains(search) ||
                        s.Patronymic.Contains(search)
                        )));

            return Ok(rezult.Select(s => (UserDTO)s));
        }

        [Authorize(Roles = "Директор отдела, Заместитель директора, Админ")]
        [HttpPost("AddNewUser")]
        public ActionResult<UserDTO> AddNewUser(UserDTO user)
        {
            var user1 = dbContext.Users.FirstOrDefault(s => s.Email == user.Email);
            if (user1 == null)
            {
                string str = GetPassword.GetPass();
                var claim = HttpContext.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier);
                int idFrom = int.Parse(claim.Value);
                UserDTO from = (UserDTO)dbContext.Users.Find(idFrom);
                //user.IdRole = 3;
                bool mailResult = PostPassword.PostPass(user, str, from);
                if (!mailResult)
                    return BadRequest("Неверная электронная почта!");
                user.Password = Md5.HashPassword(str);
                dbContext.Users.Add((User)user);
                dbContext.SaveChanges();
                return Ok((UserDTO)dbContext.Users.Include(s => s.IdRoleNavigation).Include(s => s.IdDepartmentNavigation).ThenInclude(s => s.InverseIdMainDepNavigation).FirstOrDefault(s => s.Email == user.Email));
            }
            else
            {
                return BadRequest("Пользователь с такой электронной почтой уже существует!");
            }

        }

        [Authorize(Roles = "Директор отдела, Заместитель директора, Админ")]
        [HttpPut("UpdateUser")]
        public ActionResult UpdateUser(UserDTO user)
        {
            dbContext.Users.Update((User)user);
            dbContext.SaveChanges();
            return Ok("Пользователь успешно обновлен!");
        }

        [Authorize(Roles = "Директор отдела, Заместитель директора, Админ")]
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var department = await dbContext.Users.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            dbContext.Users.Remove(department);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Директор отдела, Заместитель директора, Сотрудник")]
        [HttpPut("ChangePassword")]
        public void ChangePassword(UserDTO user)
        {
            dbContext.Users.Update((User)user);
            dbContext.SaveChanges();
        }

        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = dbContext.Users.Include(s => s.IdRoleNavigation).Include(s => s.IdDepartmentNavigation).ThenInclude(s => s.InverseIdMainDepNavigation).Where(s => s.IsDeleted == false).OrderByDescending(s => s.Id).ToList();
            return users.Select(s => (UserDTO)s);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return (UserDTO)user;
        }
    }
}
