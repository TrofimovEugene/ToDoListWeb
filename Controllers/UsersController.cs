using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
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

        public class Response
        {
	        public string access_token { get; set; }
	        public string username { get; set; }
	        public int idStudent { get; set; }
        }

        [HttpPost("UserToken")]
        public async Task<ActionResult<Response>> StudentToken(string email, string password)
        {
	        var identity = GetIdentity(email, password);
	        if (identity == null)
	        {
		        return BadRequest(new { errorText = "Invalid email or password." });
	        }

	        var now = DateTime.UtcNow;
	        // создаем JWT-токен
	        var jwt = new JwtSecurityToken(
		        issuer: AuthOptions.ISSUER,
		        audience: AuthOptions.AUDIENCE,
		        notBefore: now,
		        claims: identity.Claims,
		        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
		        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
	        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

	        var users = await _context.User.ToListAsync();

	        return (from user in users
			        where user.Email == identity.Name
			        select new Response {access_token = encodedJwt, username = identity.Name, idStudent = user.Id})
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

	        // если пользователя не найдено
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
