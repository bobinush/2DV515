using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;

namespace mvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApiDbContext _context;

        public UsersController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Users.AsNoTracking().ToList());
        }

        [HttpGet("[controller]/{id}")]
        public IActionResult View(int id)
        {

            var user = _context.Users
                .Include(x => x.Ratings).ThenInclude(x => x.Movie)
                .SingleOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound();

            UserViewModel model = DoWork(user);
            model.Ratings = user.Ratings;

            return View(model);
        }

        private UserViewModel DoWork(User selectedUser)
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
                similarUser.Pearson.Score = user.CalcPearson(selectedUser);
                similarUser.Euclidean.Score = user.CalcEuclidean(selectedUser);

                // Only process the movies not seen by the selected user
                var ratings = user.Ratings.Where(x => selectedUser.Ratings.All(y => y.MovieId != x.MovieId));
                foreach (var rating in ratings)
                {
                    // Calc weighted movie score (similarity * rating)
                    similarUser.Pearson.Movies.Add(new MovieViewModel()
                    {
                        MovieId = rating.MovieId,
                        Title = rating.Movie.Title,
                        Score = similarUser.Pearson.Score * rating.Score,
                    });
                    similarUser.Euclidean.Movies.Add(new MovieViewModel()
                    {
                        MovieId = rating.MovieId,
                        Title = rating.Movie.Title,
                        Score = similarUser.Euclidean.Score * rating.Score,
                    });
                }
                similarUsers.Add(similarUser);
            }

            // Sum all weighted movie score 
            // Sum user similarity for each movie
            var movierecPearson = similarUsers.SelectMany(u => u.Pearson.Movies)
                .GroupBy(m => new { m.MovieId, m.Title })
                .Select(x => new MovieViewModel()
                {
                    MovieId = x.Key.MovieId,
                    Title = x.Key.Title,
                    Score = x.Sum(s => s.Score), // Sum weighted movie score
                    SimilarityScore = similarUsers // Sum similarity score
                        .Where(u => u.Pearson.Movies.Any(m => m.MovieId == x.Key.MovieId))
                        .Sum(s => s.Pearson.Score),
                }).ToList();

            var movierecEuclidean = similarUsers.SelectMany(u => u.Euclidean.Movies)
                .GroupBy(m => new { m.MovieId, m.Title })
                .Select(x => new MovieViewModel()
                {
                    MovieId = x.Key.MovieId,
                    Title = x.Key.Title,
                    Score = x.Sum(s => s.Score), // Sum weighted movie score
                    SimilarityScore = similarUsers // Sum similarity score
                        .Where(u => u.Euclidean.Movies.Any(m => m.MovieId == x.Key.MovieId))
                        .Sum(s => s.Euclidean.Score),
                }).ToList();

            // TODO : Lägg till grafiskt gränssnitt, gör en MVC med dropdownlista över användare, radiobutton för euclidean/pearson 

            Pearson p = new Pearson()
            {
                SimilarUsers = similarUsers
                    .Select(x => new UserViewModel()
                    {
                        Score = x.Pearson.Score
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList(),
                Movies = movierecPearson
                    .Select(x => new MovieViewModel()
                    {
                        MovieId = x.MovieId,
                        Title = x.Title,
                        Score = x.Score / x.SimilarityScore
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList()
            };

            Euclidean e = new Euclidean()
            {
                SimilarUsers = similarUsers
                    .Select(x => new UserViewModel()
                    {
                        Score = x.Euclidean.Score
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList(),
                Movies = movierecEuclidean
                    .Select(x => new MovieViewModel()
                    {
                        MovieId = x.MovieId,
                        Title = x.Title,
                        Score = x.Score / x.SimilarityScore
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList()
            };

            return new UserViewModel()
            {
                Id = selectedUser.Id,
                Name = selectedUser.Name,
                Pearson = p,
                Euclidean = e
            };
        }
    }
}
