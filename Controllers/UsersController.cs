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
            return Json(_context.Users.Include(x => x.Ratings).ToList());
        }

        // GET api/users/pearson/5
        [HttpGet("pearson/{id}")]
        public JsonResult GetPearson(int id)
        {
            User selectedUser = _context.Users.Include(x => x.Ratings).SingleOrDefault(x => x.Id == id);
            if (selectedUser == null)
                return Json(NotFound());

            UserViewModel result = DoWork(selectedUser, true);
            return Json(result);
        }

        // GET api/users/euclidean/5
        [HttpGet("euclidean/{id}")]
        public JsonResult GetEuclidean(int id)
        {
            User selectedUser = _context.Users.Include(x => x.Ratings).SingleOrDefault(x => x.Id == id);
            if (selectedUser == null)
                return Json(NotFound());

            UserViewModel result = DoWork(selectedUser, false);
            return Json(result);
        }

        private UserViewModel DoWork(User selectedUser, bool doPearson)
        {
            List<User> otherUsers = _context.Users
                .Include(x => x.Ratings)
                .ThenInclude(r => r.Movie)
                .Where(x => x.Id != selectedUser.Id)
                .ToList();

            var similarUsers = new List<UserViewModel>();
            foreach (var user in otherUsers)
            {
                var similarUser = new UserViewModel(user);
                // Calc similarity score
                if (doPearson)
                    similarUser.Score = user.CalcPearson(selectedUser);
                else
                    similarUser.Score = user.CalcEuclidean(selectedUser);

                // Only process the movies not seen by the selected user
                var ratings = user.Ratings.Where(x => selectedUser.Ratings.All(y => y.MovieId != x.MovieId));
                foreach (var rating in ratings)
                {
                    // Calc weighted movie score (similarity * rating)
                    similarUser.Movies.Add(new MovieViewModel()
                    {
                        MovieId = rating.MovieId,
                        Title = rating.Movie.Title,
                        Score = similarUser.Score * rating.Score
                    });
                }
                similarUsers.Add(similarUser);
            }

            // Sum all weighted movie score 
            // Sum user similarity for each movie
            var movierec = similarUsers.SelectMany(u => u.Movies)
                .GroupBy(m => new { m.MovieId, m.Title })
                .Select(x => new MovieViewModel()
                {
                    MovieId = x.Key.MovieId,
                    Title = x.Key.Title,
                    Score = x.Sum(s => s.Score), // Sum weighted movie score
                    SimilarityScore = similarUsers // Sum similarity score
                        .Where(u => u.Movies.Any(m => m.MovieId == x.Key.MovieId))
                        .Sum(s => s.Score),
                }).ToList();

            return new UserViewModel()
            {
                Id = selectedUser.Id,
                Name = selectedUser.Name,
                SimilarUsers = similarUsers
                    .Select(x => new UserViewModel() { Id = x.Id, Name = x.Name, Score = x.Score })
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList(),
                Movies = movierec
                    .Select(x => new MovieViewModel() { MovieId = x.MovieId, Title = x.Title, Score = x.Score / x.SimilarityScore })
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList()
            };
        }
    }
}
