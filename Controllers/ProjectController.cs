using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;

namespace mvc.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApiDbContext _context;

        public ProjectController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? p)
        {
            int pageSize = 15;
            var model = await PaginatedList<UserP>.CreateAsync(
                _context.UsersP.AsNoTracking(), p ?? 1, pageSize);
            return View(model);
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> View(int id, int? page)
        {
            var user = _context.UsersP
                .Include(x => x.Ratings).ThenInclude(x => x.Movie)
                .SingleOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound();

            var model = new UserViewModel(user);
            int pageSize = 8;
            model.RatingsP = await PaginatedList<RatingP>.CreateAsync(
                            _context.RatingsP.AsNoTracking()
                                .Include(x => x.Movie)
                                .Where(x => x.UserId == id)
                                .OrderByDescending(t => t.Timestamp),
                            page ?? 1, pageSize);

            if (page != null)
                return PartialView("_Ratings", model);
            else
                return View(model);
        }

        public IActionResult CalcPearson(int id, int minRatings = 2)
        {
            var user = _context.UsersP
                            .Include(x => x.Ratings).ThenInclude(x => x.Movie)
                            .SingleOrDefault(x => x.Id == id);

            UserViewModel model = DoWork(user, true, minRatings);
            return PartialView("_Recommendations", model.Distance.Movies);
        }

        public IActionResult CalcEuclidean(int id, int minRatings = 2)
        {
            var user = _context.UsersP
                .Include(x => x.Ratings).ThenInclude(x => x.Movie)
                .SingleOrDefault(x => x.Id == id);

            UserViewModel model = DoWork(user, false, minRatings);

            return PartialView("_Recommendations", model.Distance.Movies);
        }

        private UserViewModel DoWork(UserP selectedUser, bool pearson, int minRatings)
        {
            List<UserP> otherUsers = _context.UsersP
                .Include(x => x.Ratings)
                .ThenInclude(r => r.Movie)
                .Where(x => x.Id != selectedUser.Id)
                .ToList();

            var similarUsers = new List<UserViewModel>();
            foreach (var user in otherUsers)
            {
                var similarUser = new UserViewModel(user);
                // Calc similarity score
                if (pearson)
                    similarUser.Distance.Score = user.CalcPearson(selectedUser, minRatings);
                else
                    similarUser.Distance.Score = user.CalcEuclidean(selectedUser, minRatings);

                // Only include users with similarity of more than 0 in the calculations
                if (similarUser.Distance.Score < 0)
                    continue;

                // Only process the movies not seen by the selected user
                var ratings = user.Ratings.Where(x => selectedUser.Ratings.All(y => y.MovieId != x.MovieId));
                foreach (var rating in ratings)
                {
                    // Calc weighted movie score (similarity * rating)
                    similarUser.Distance.Movies.Add(new MovieViewModel()
                    {
                        MovieId = rating.MovieId,
                        Title = rating.Movie.Title,
                        Score = similarUser.Distance.Score * rating.Score,
                    });
                }
                similarUsers.Add(similarUser);
            }

            // Sum all weighted movie score 
            // Sum user similarity for each movie
            var movierecDistance = similarUsers.SelectMany(u => u.Distance.Movies)
                .GroupBy(m => new { m.MovieId, m.Title })
                .Select(x => new MovieViewModel()
                {
                    MovieId = x.Key.MovieId,
                    Title = x.Key.Title,
                    Score = x.Sum(s => s.Score), // Sum weighted movie score
                    SimilarityScore = similarUsers // Sum similarity score
                        .Where(u => u.Distance.Movies.Any(m => m.MovieId == x.Key.MovieId))
                        .Sum(s => s.Distance.Score),
                }).ToList();

            var d = new DistanceMetric()
            {
                SimilarUsers = similarUsers
                     .Select(x => new UserViewModel()
                     {
                         Score = x.Distance.Score
                     })
                     .OrderByDescending(x => x.Score)
                     .Take(3).ToList(),
                Movies = movierecDistance
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
                Distance = d
            };
        }
    }
}
