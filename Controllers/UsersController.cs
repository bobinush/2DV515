using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context)
        {
            _context = context;
        }

        // GET api/users
        [HttpGet]
        public JsonResult GetAll()
        {
            return Json(_context.Users.ToList());
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            User selectedUser = _context.Users.Include(x => x.Ratings).SingleOrDefault(x => x.Id == id);
            if (selectedUser == null)
            {
                return Json(NotFound());
            }

            IEnumerable<User> users = _context.Users.Include(x => x.Ratings).Where(x => x.Id != id);
            var similarUsers = new List<UserViewModel>();

            foreach (var user in users)
            {
                similarUsers.Add(new UserViewModel(user)
                {
                    Euclidean = user.CalcEuclidean(selectedUser),
                    Pearson = user.CalcPearson(selectedUser)
                });
            }

            return Json(new
            {
                selectedUser.Id,
                selectedUser.Name,
                Euclidean = similarUsers
                    .Select(x => new { x.Id, x.Name, x.Euclidean })
                    .OrderByDescending(x => x.Euclidean)
                    .Take(3),
                Pearson = similarUsers
                    .Select(x => new { x.Id, x.Name, x.Pearson })
                    .OrderByDescending(x => x.Pearson)
                    .Take(3)
            });
        }

        // // POST api/users
        // [HttpPost]
        // public void Post([FromBody] string value)
        // {
        // }

        // // PUT api/users/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }

        // // DELETE api/users/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
