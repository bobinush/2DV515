using System;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using TinyCsvParser;
using webapi.Models;

namespace webApi.Models
{
    public static class DbInitializer
    {
        public static void Initialize(ApiDbContext context)
        {
            // context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
            InitA1(context);
            InitA2(context);

        }

        public static void InitA1(ApiDbContext context)
        {
            var parserOptions = new CsvParserOptions(skipHeader: true, fieldsSeparator: ';');

            var userParser = new CsvParser<User>(parserOptions, new UserClassMap());
            var users = userParser.ReadFromFile("users.csv", Encoding.UTF8).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                context.Users.Add(users[i].Result);
            }

            var ratingParser = new CsvParser<Rating>(parserOptions, new RatingClassMap());
            var ratings = ratingParser.ReadFromFile("ratings.csv", Encoding.UTF8).ToList();
            var movies = ratings.Select(x => x.Result.Title).Distinct().ToList();
            for (int i = 0; i < movies.Count; i++)
            {
                context.Movies.Add(new Movie() { Title = movies[i] });
            }
            context.SaveChanges();

            var m = context.Movies.ToList();
            for (int i = 0; i < ratings.Count; i++)
            {
                ratings[i].Result.MovieId = m.Find(x => x.Title == ratings[i].Result.Title).Id;
                context.Ratings.Add(ratings[i].Result);
            }
            context.SaveChanges();
        }

        public static void InitA2(ApiDbContext context)
        {
            string path = "blogdata.txt";
            using (TextReader fileReader = System.IO.File.OpenText(path))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.BadDataFound = null;
                csv.Configuration.Delimiter = "\t";
                csv.Configuration.IgnoreQuotes = true;
                csv.Configuration.BadDataFound = c =>
                {
                    System.Console.WriteLine($"Bad data found on row '{c.RawRow} {c.RawRecord}'");
                };
                var records = csv.GetRecords<dynamic>().ToList();

                // From dynamic to the class Blog with a dictionary<string, int> Words
                for (int i = 0; i < records.Count(); i++)
                {
                    var b = new Blog();
                    foreach (var blog in records[i])
                    {
                        if (blog.Key == "Blog")
                            b.Name = blog.Value;
                        else
                            b.Words.Add(new Word() { Key = blog.Key, Value = double.Parse(blog.Value) });
                    }
                    context.Blogs.Add(b);
                }
            }
            context.SaveChanges();
        }
    }
}