using System;
using System.Linq;
using System.Text;
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

            // Create a new User if collection is empty,
            // which means you can't delete all Users.
            var parserOptions = new CsvParserOptions(skipHeader: true, fieldsSeparator: ';');

            var userParser = new CsvParser<User>(parserOptions, new UserClassMap());
            var users = userParser.ReadFromFile("users.csv", Encoding.UTF8).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                context.Users.Add(users[i].Result);
            }

            var ratingParser = new CsvParser<Rating>(parserOptions, new RatingClassMap());
            var ratings = ratingParser.ReadFromFile("ratings.csv", Encoding.UTF8).ToList();
            for (int i = 0; i < ratings.Count; i++)
            {
                context.Ratings.Add(ratings[i].Result);
            }
            context.SaveChanges();
        }
    }
}