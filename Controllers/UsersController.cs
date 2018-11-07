using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context)
        {
            _context = context;
            if (_context.Users.Count() == 0)
            {
                // Create a new User if collection is empty,
                // which means you can't delete all Users.

                _context.Users.Add(new User { Name = "Test" });
                _context.SaveChanges();
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            return _context.Users.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
