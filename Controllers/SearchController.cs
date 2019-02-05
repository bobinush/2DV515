using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using CsvHelper;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Reflection;

namespace mvc.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApiDbContext _context;
        private readonly double HIGH_VALUE = 100000;

        public SearchController(ApiDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Top 5 results</returns>
        [HttpGet]
        public IActionResult Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new List<ScoreViewModel>());

            List<ScoreViewModel> model = OrderByContent(query);

            return View(model.Take(5).ToList());
        }

        // Content-Based Ranking 
        private List<ScoreViewModel> OrderByContent(string query)
        {
            var result = new List<ScoreViewModel>();
            int[] q = query.Split().Select(x => GetIdForWord(x)).Where(x => x != 0).ToArray();
            if (q.Length == 0)
                return result;

            // Load all pages into memory because getting all one by one takes
            // forever with the current database setup
            var pages = _context.Pages.Include(x => x.Words).AsNoTracking().ToList();
            int numberOfPages = pages.Count();
            var scores = new MetricsViewModel(numberOfPages);

            // Calculate score for each page
            for (int i = 0; i < numberOfPages; i++)
            {
                Page p = pages.Find(x => x.ID == i + 1);
                scores.Content[i] = getFrequencyScore(p, q);
                scores.Location[i] = getLocationScore(p, q);
                // if (q.Length > 1)
                //     scores.Distance[i] = getDistanceScore(p, q);
            }

            // Normalize scores
            normalize(scores.Content, false);
            normalize(scores.Location, true);
            // normalize(scores.Distance, true);

            // Generate result list
            for (int i = 0; i < numberOfPages; i++)
            {
                Page p = pages.Find(x => x.ID == i + 1);
                result.Add(new ScoreViewModel(p, scores.Content[i], scores.Location[i]));
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
            foreach (var q in query)
            {
                foreach (var word in p.Words)
                {
                    if (q == word.Value)
                    {
                        score += (p.Words.IndexOf(word) + 1); // IndexOf = zero-based
                        break;
                    }
                }
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

                score += Math.Abs(score1 - score2); // 0 - 1000 = 1000
            }

            return score;
        }

        private int GetIdForWord(string word)
        {
            var w = _context.WordMap.SingleOrDefault(x => x.Key == word);
            if (w != null)
                return w.Id;
            else
                return 0;
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
                    scores[i] = Math.Round(min / Math.Max(scores[i], 0.00001), 5);
            }
            else
            {
                // Higher values shall be scaled between 0 and 1
                double max = scores.Max();
                // When we have a max value, divide all score by it
                for (int i = 0; i < scores.Length; i++)
                    scores[i] = Math.Round(scores[i] / max, 5);
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
