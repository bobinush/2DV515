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
    public class RatingsController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public RatingsController(ApiDbContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Rating>> GetAll()
        {
            return _context.Ratings.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Rating> Get(int id)
        {
            var rating = _context.Ratings.Find(id);
            if (rating == null)
            {
                return NotFound();
            }
            return rating;
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
