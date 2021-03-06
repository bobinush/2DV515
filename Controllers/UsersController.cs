﻿using System;
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

            var model = new UserViewModel(user);
            return View(model);
        }

        [HttpGet]
        public IActionResult CalcPearson(int id, int minRatings = 2)
        {
            var user = _context.Users
                            .Include(x => x.Ratings).ThenInclude(x => x.Movie)
                            .SingleOrDefault(x => x.Id == id);

            UserViewModel model = DoWork(user, true);
            return PartialView("_Recommendations", model.Distance.Movies);
        }

        [HttpGet]
        public IActionResult CalcEuclidean(int id, int minRatings = 2)
        {
            var user = _context.Users
                .Include(x => x.Ratings).ThenInclude(x => x.Movie)
                .SingleOrDefault(x => x.Id == id);

            UserViewModel model = DoWork(user, false);

            return PartialView("_Recommendations", model.Distance.Movies);
        }

        private UserViewModel DoWork(User selectedUser, bool pearson)
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
                if (pearson)
                    similarUser.Distance.Score = user.CalcPearson(selectedUser);
                else
                    similarUser.Distance.Score = user.CalcEuclidean(selectedUser);

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
