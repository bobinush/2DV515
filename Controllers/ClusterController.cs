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

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClusterController : Controller
    {
        private readonly ApiDbContext _context;

        public ClusterController(ApiDbContext context)
        {
            _context = context;
        }

        // GET api/cluster
        /// <summary>
        /// K-Means clustering, will stop when  no new assignments are made.
        /// </summary>
        /// <param name="maxIterations">Number of max iterations</param>
        /// <returns></returns>
        [HttpGet("KMeans/{maxIterations}")]
        public JsonResult KMeans(int maxIterations = 30)
        {
            int iterations = 0;
            int clusters = 5;
            var centroids = new List<CentroidViewModel>();
            var words = _context.Blogs.Include(x => x.Words).First().Words.ToList();
            var blogWords = _context.Blogs.Include(x => x.Words).SelectMany(x => x.Words).ToList();
            var rnd = new Random();
            bool newMatchesFound = true;
            dynamic d = new List<ExpandoObject>();

            // Get min/max of all words
            for (int i = 0; i < words.Count; i++)
            {
                dynamic e = new ExpandoObject();
                e.Word = words[i].Key;
                e.Min = blogWords.Where(x => x.Key == words[i].Key).Min(x => x.Value);
                e.Max = blogWords.Where(x => x.Key == words[i].Key).Max(x => x.Value);
                d.Add(e);
            }
            // Generate "random" value of all words for all centroids
            for (int k = 0; k < clusters; k++)
            {
                var c = new CentroidViewModel() { Name = "Centroid " + k };
                for (int i = 0; i < d.Count; i++)
                {
                    var word = d[i].Word;
                    var min = d[i].Min;
                    var max = d[i].Max;
                    c.Words.Add(new Word() { Key = word, Value = rnd.Next((int)min, (int)max) });
                }
                centroids.Add(c);
            }

            while (newMatchesFound && iterations < maxIterations)
            {
                centroids.ForEach(x => x.ClearAssignments());
                foreach (Blog b in _context.Blogs.Include(x => x.Words))
                {
                    double distance = double.MaxValue;
                    var best = new CentroidViewModel();

                    // Find closest centroid
                    foreach (var c in centroids)
                    {
                        double cDist = c.Pearson(b);
                        if (cDist < distance)
                        {
                            best = c;
                            distance = cDist;
                        }
                    }
                    best.Blogs.Add(b);
                }

                // Re-calculate center for each centroid
                foreach (var c in centroids)
                {
                    for (int j = 0; j < words.Count - 1; j++)
                    {
                        var word = words[j].Key;
                        double avg = 0;
                        // Iterate over all blogs assigned to this centroid to find average 
                        foreach (var b in c.Blogs)
                        {
                            avg += b.Words.Where(x => x.Key == word).Sum(x => x.Value);
                        }
                        avg /= c.Blogs.Count;

                        c.Words.First(x => x.Key == word).Value = avg;
                    }
                }

                iterations++;
                newMatchesFound = !centroids.All(x => x.IsClusterSameAsPrevious());
                // System.Console.WriteLine(centroids.First().Words.First().Value);
                // System.Console.WriteLine("-- iteration " + iterations);
                // centroids.Select(x => new { x.NumberOfBlogs, PreviousBlogs = x.PreviousBlogs.Count }).ToList().ForEach(x => System.Console.WriteLine(x));
            }
            return Json(centroids.Select(x => new
            {
                x.Name,
                x.NumberOfBlogs,
                Blogs = x.Blogs.Select(b => b.Name)
            }));
        }
    }
}
