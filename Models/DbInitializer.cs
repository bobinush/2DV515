using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using TinyCsvParser;

namespace mvc.Models
{
    public class DbInitializer
    {
        private readonly ApiDbContext _context;
        private Dictionary<string, int> dic = new Dictionary<string, int>();

        public DbInitializer(ApiDbContext context)
        {
            _context = context;

        }
        public void Initialize()
        {
            _context.Database.EnsureCreated();

            if (_context.MoviesP.Any())
            {
                return;   // DB has been seeded
            }

            InitA1();
            InitA2();
            InitA3();
            InitProject();
        }

        public void InitA1()
        {
            var parserOptions = new CsvParserOptions(skipHeader: true, fieldsSeparator: ';');

            var userParser = new CsvParser<User>(parserOptions, new UserClassMap());
            var users = userParser.ReadFromFile("users.csv", Encoding.UTF8).ToList();
            _context.BulkInsert(users.Select(x => x.Result));

            var ratingParser = new CsvParser<Rating>(parserOptions, new RatingClassMap());
            var ratings = ratingParser.ReadFromFile("ratings.csv", Encoding.UTF8).ToList();
            var movies = ratings.Select(x => x.Result.Title).Distinct().ToList();
            _context.BulkInsert(movies.Select(x => new Movie() { Title = x }));

            var m = _context.Movies.ToList(); // To get the assigned ID
            for (int i = 0; i < ratings.Count; i++)
                ratings[i].Result.MovieId = m.Find(x => x.Title == ratings[i].Result.Title).Id;

            _context.BulkInsert(ratings.Select(x => x.Result));
        }

        public void InitA2()
        {
            string path = "blogdata.txt";
            var blogList = new List<Blog>();
            var blogWordList = new List<BlogWord>();
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
                    b.Id = i + 1;
                    foreach (var blog in records[i])
                    {
                        if (blog.Key == "Blog")
                            b.Name = blog.Value;
                        else
                            blogWordList.Add(new BlogWord() { Key = blog.Key, Value = double.Parse(blog.Value), BlogId = b.Id });
                    }
                    blogList.Add(b);
                }
            }
            _context.BulkInsert(blogList);
            _context.BulkInsert(blogWordList);
        }

        public void InitA3()
        {
            int id;
            string path = "wikipedia/Words";
            string[] files = Directory.GetFiles(path, "", SearchOption.AllDirectories);
            var wordsList = new List<PageWord>();
            var wordList = new List<WordMap>();
            var pages = new List<Page>();
            for (int j = 0; j < files.Length; j++)
            {
                var text = File.ReadAllText(files[j]);
                var words = text.Split(" ");
                var page = new Page();
                var pageDic = new Dictionary<string, int>();
                page.Url = files[j].Replace("wikipedia/Words", "");
                page.ID = j + 1;

                // From dynamic to the class Blog with a dictionary<string, int> Words
                for (int i = 0; i < words.Length; i++)
                {
                    id = GetIdForWord(words[i]);
                    pageDic[words[i]] = id;
                }
                foreach (var item in pageDic)
                    wordsList.Add(new PageWord { Value = item.Value, PageId = page.ID });

                pages.Add(page);
            }

            foreach (var item in dic)
                wordList.Add(new WordMap { Key = item.Key });

            _context.BulkInsert(pages);
            _context.BulkInsert(wordsList);
            _context.BulkInsert(wordList);
        }

        private int GetIdForWord(string word)
        {
            if (dic.ContainsKey(word))
                return dic[word];
            else
            {
                int id = dic.Count + 1;
                dic.Add(word, id);
                return id;
            }
        }

        private void InitProject()
        {
            string folder = "MovieLens/";
            string[] paths = { "movies.csv", "links.csv", "ratings.csv", "tags.csv" };
            var movies = new List<MovieP>();
            var movieGenres = new List<MovieGenreP>();
            var genres = new HashSet<GenreP>();
            var ratings = new List<RatingP>();
            var tags = new List<TagP>();
            var users = new HashSet<UserP>();
            foreach (var path in paths)
            {
                using (TextReader fileReader = System.IO.File.OpenText(folder + path))
                {
                    var csv = new CsvReader(fileReader);
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.BadDataFound = null;
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.IgnoreQuotes = false;
                    csv.Configuration.BadDataFound = c =>
                    {
                        System.Console.WriteLine($"Bad data found on row '{c.RawRow} {c.RawRecord}'");
                    };

                    switch (path)
                    {
                        case "movies.csv":
                            var anonymousMovie = new
                            {
                                movieId = default(int),
                                title = string.Empty,
                                genres = string.Empty
                            };
                            var recordsMovies = csv.GetRecords(anonymousMovie);
                            foreach (var item in recordsMovies)
                            {
                                movies.Add(new MovieP() { Id = item.movieId, Title = item.title });
                                var g = item.genres.Split("|").Select(x => new GenreP { Title = x });
                                int i = 0;
                                foreach (var genre in g)
                                {
                                    genres.Add(genre);
                                    movieGenres.Add(new MovieGenreP { MovieId = item.movieId, GenreId = ++i });
                                }
                            }

                            break;
                        case "links.csv":
                            var anonymousLink = new
                            {
                                movieId = default(int),
                                imdbId = string.Empty,
                                tmdbId = string.Empty
                            };
                            var recordsLinks = csv.GetRecords(anonymousLink);
                            foreach (var item in recordsLinks)
                            {
                                var m = movies.SingleOrDefault(x => x.Id == item.movieId);
                                if (m == null)
                                    continue;
                                m.Imdb = item.imdbId;
                                m.Tmdb = item.tmdbId;
                            }
                            break;
                        case "ratings.csv":
                            var anonymousRating = new
                            {
                                movieId = default(int),
                                userId = default(int),
                                rating = string.Empty,
                                timestamp = default(int)
                            };
                            var recordsRatings = csv.GetRecords(anonymousRating);
                            foreach (var item in recordsRatings)
                            {
                                users.Add(new UserP { Id = item.userId });
                                ratings.Add(new RatingP
                                {
                                    UserId = item.userId,
                                    MovieId = item.movieId,
                                    Score = double.Parse(item.rating, System.Globalization.CultureInfo.InvariantCulture), // decimal symbol
                                    Timestamp = item.timestamp
                                });
                            }

                            // Update number of ratings
                            movies.ForEach(x => x.NumberOfRatings = ratings.Where(r => r.MovieId == x.Id).Count());
                            break;
                        case "tags.csv":
                            var anonymousTag = new
                            {
                                movieId = default(int),
                                userId = default(int),
                                tag = string.Empty,
                                timestamp = default(int)
                            };
                            var recordsTags = csv.GetRecords(anonymousTag);
                            foreach (var item in recordsTags)
                            {
                                users.Add(new UserP { Id = item.userId });
                                tags.Add(new TagP
                                {
                                    UserId = item.userId,
                                    MovieId = item.movieId,
                                    Tag = item.tag,
                                    Timestamp = item.timestamp
                                });
                            }

                            break;
                    }
                }
            }
            _context.BulkInsert(genres);
            _context.BulkInsert(movies);
            _context.BulkInsert(users);
            _context.BulkInsert(ratings);
            _context.BulkInsert(tags);
            _context.BulkInsert(movieGenres);
        }
    }
}