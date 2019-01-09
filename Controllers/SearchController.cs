using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
using CsvHelper;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Reflection;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly ApiDbContext _context;
        private readonly double HIGH_VALUE = 10000;

        public SearchController(ApiDbContext context)
        {
            _context = context;
        }

        // GET api/search
        /// <summary>
        /// Search
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Top 5 results</returns>
        [HttpGet("{query}")]
        public JsonResult Index(string query)
        {
            var model = OrderByContent(query);

            return Json(model.Take(5));
        }

        // Content-Based Ranking 
        private List<ScoreViewModel> OrderByContent(string query)
        {
            // Load all pages into memory because getting all one by one takes
            // forever with the current database setup
            var pages = _context.Pages.Include(x => x.Words).AsNoTracking().ToList();
            int numberOfPages = _context.Pages.Count();
            var result = new List<ScoreViewModel>();
            var scores = new MetricsViewModel(numberOfPages);
            int[] q = query.Split().Select(x => GetIdForWord(x)).ToArray();

            // Calculate score for each page
            for (int i = 1; i < numberOfPages + 1; i++)
            {
                Page p = pages.Find(x => x.ID == i);
                scores.Content[i - 1] = getFrequencyScore(p, q);
                scores.Location[i - 1] = getLocationScore(p, q);
                if (q.Length > 1)
                    scores.Distance[i - 1] = getDistanceScore(p, q);
            }

            // Normalize scores
            normalize(scores.Content, false);
            normalize(scores.Location, true);
            normalize(scores.Distance, true);

            // Generate result list
            for (int i = 1; i < numberOfPages + 1; i++)
            {
                Page p = pages.Find(x => x.ID == i);
                double score = 1.0 * scores.Content[i - 1]
                    + 0.8 * scores.Location[i - 1]
                    + 0.5 * scores.Distance[i - 1];
                result.Add(new ScoreViewModel(p, score));
            }

            // Sort result list with highest score first
            result.Sort((a, b) => b.Score.CompareTo(a.Score));

            return result;
        }

        // Word Frequency metric
        private double getFrequencyScore(Page p, int[] query)
        {
            double score = 0;
            foreach (var word in p.Words)
            {
                if (query.Contains(word.Value))
                    score++;
            }
            return score;
        }

        // Document Location metric
        private double getLocationScore(Page p, int[] query)
        {
            double score = 0;
            foreach (var word in p.Words)
            {
                if (query.Contains(word.Value))
                    score += p.Words.IndexOf(word) + 1; // IndexOf = zero-based
            }
            return score == 0 ? HIGH_VALUE : score;
        }

        // Word Distance metric
        private double getDistanceScore(Page p, int[] query)
        {
            double score = 0;
            var pairs = GetKCombs(query, 2);
            foreach (var pair in pairs)
            {
                double score1 = getLocationScore(p, new[] { pair.First() });
                double score2 = getLocationScore(p, new[] { pair.Last() });
                // If any or both words are missing in the document
                if (score1 == HIGH_VALUE && score1 == score2)
                    score1 = 0;

                score += Math.Abs(score1 - score2);
            }

            return score;
        }

        private int GetIdForWord(string word)
        {
            var w = _context.WordMap.SingleOrDefault(x => x.Key == word);
            if (w != null)
                return w.ID;
            else
            {
                int id = _context.WordMap.Last().ID + 1;
                var newWord = new WordMap { Key = word };
                _context.WordMap.Add(newWord);
                _context.SaveChanges();
                return newWord.ID;
            }
        }

        private void normalize(double[] scores, bool smallIsBetter)
        {
            if (smallIsBetter)
            {
                // Smaller values shall be inverted to higher values
                // and scaled between 0 and 1
                double min = scores.Min();
                // Divide the min value by the score (and avoid division by zero)
                for (int i = 0; i < scores.Length; i++)
                    scores[i] = min / Math.Max(scores[i], 0.00001);
            }
            else
            {
                // Higher values shall be scaled between 0 and 1
                double max = scores.Max();
                // When we have a max value, divide all score by it
                for (int i = 0; i < scores.Length; i++)
                    scores[i] = scores[i] / max;
            }
        }

        // https://stackoverflow.com/a/10629938/1695663
        private IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
