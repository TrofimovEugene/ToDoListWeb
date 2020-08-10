using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoListWeb.Data;
using ToDoListWeb.Models;

namespace ToDoListWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ToDoListWebContext _context;

        public UsersController(ToDoListWebContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        /// <summary>
        /// Редактирование информации о пользователе.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditingUserInformation(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        /// <summary>
        /// Функция регистрации пользователя в системе.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// Возвращает JSON c данными зарегистрированного пользователя в системе.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserById", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        /// <summary>
        /// Удаление пользователя из системы
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Возвращает JSON c данными удалённого пользователя.
        /// </returns>
        [HttpDelete("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public new class Request
		{
            public string Email { get; set; }
            public string Password { get; set; }
		}

        public new class Response
        {
	        public string AccessToken { get; set; }
	        public string Email { get; set; }
	        public int IdUser { get; set; }
        }

        [HttpPost("AutorizationUserAndGetToken")]
        public async Task<ActionResult<Response>> AutorizationUserAndGetToken(Request request)
        {
	        var identity = GetIdentity(request.Email, request.Password);
	        if (identity == null)
	        {
		        return BadRequest(new { errorText = "Invalid email or password." });
	        }

	        var now = DateTime.UtcNow;
	        // создаем JWT-токен
	        var jwt = new JwtSecurityToken(
		        issuer: AuthOptions.Issuer,
		        audience: AuthOptions.Audience,
		        notBefore: now,
		        claims: identity.Claims,
		        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
		        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
	        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

	        var users = await _context.User.ToListAsync();

	        return (from user in users
			        where user.Email == identity.Name
			        select new Response {AccessToken = encodedJwt, Email = identity.Name, IdUser = user.Id})
		        .FirstOrDefault();
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
	        var users = _context.User.ToList();
	        User user = null;
	        foreach (var elem in users.Where(elem => elem.Email == email).Where(elem => elem.Password == password))
	        {
		        user = elem;
	        }
	        // если пользователя не найдено
            if (user == null) return null;
	        var claims = new List<Claim>
	        {
		        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
		        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
	        };
	        var claimsIdentity =
		        new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
			        ClaimsIdentity.DefaultRoleClaimType);
	        return claimsIdentity;
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
