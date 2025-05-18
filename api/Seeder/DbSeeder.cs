using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Bogus;

namespace api.Seeder
{
    public static class DbSeeder
    {
        public static void Seeder(ApplicationDbContext context)
        {
            // 1. Generate and save Users
            var userFaker = new Faker<User>()
                .RuleFor(x => x.UserName, f => f.Person.UserName)
                .RuleFor(x => x.Email, f => f.Person.Email)
                .RuleFor(x => x.Password, f => f.Internet.Password())
                .RuleFor(x => x.About, f => f.Lorem.Paragraph())
                .RuleFor(x => x.Avatar, f => f.Internet.Avatar())
                .RuleFor(x => x.CreatedAt, f => f.Date.Past());

            var users = userFaker.Generate(100);
            context.Users.AddRange(users);
            context.SaveChanges();

            // 2. Generate and save Series
            var seriesFaker = new Faker<Series>()
                .RuleFor(s => s.Title, f => f.Lorem.Sentence(3))
                .RuleFor(s => s.Slug, (f, s) => s.Title.ToLower().Replace(" ", "-").Replace(".", "").Replace(",", "")) //this one neeeds to refer to title, but it will be llike for example naruto shippuden , becomes naruto-shippuden
                .RuleFor(s => s.Description, f => f.Lorem.Paragraph())
                .RuleFor(s => s.Thumbnail, f => f.Image.PicsumUrl())
                .RuleFor(s => s.CreatedAt, f => f.Date.Past());

            var seriesList = seriesFaker.Generate(10);
            context.Series.AddRange(seriesList);
            context.SaveChanges();


            // generate episode faker
            var episodeFaker = new Faker<Episode>()
                .RuleFor(e => e.Title, f => f.Lorem.Sentence(2))
                .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
                .RuleFor(e => e.Thumbnail, f => f.Image.PicsumUrl())
                .RuleFor(e => e.CreatedAt, f => f.Date.Past())
                .RuleFor(e => e.Season, f => f.Random.Bool() ? f.Random.Int(1, 5) : null)
                .RuleFor(e => e.EpisodeNumber, f => f.Random.Bool() ? f.Random.Int(1, 20) : null);

            var episodes = new List<Episode>();

            foreach (var series in seriesList)
            {
                int episodeCount = new Random().Next(1, 5);
                var generatedEpisodes = episodeFaker.Generate(episodeCount);

                foreach (var ep in generatedEpisodes)
                {
                    ep.SeriesId = series.Id;
                    ep.Series = series;
                }

                episodes.AddRange(generatedEpisodes);
                context.Episodes.AddRange(generatedEpisodes);

                // Now set SeriesFormat based on episodes
                if (generatedEpisodes.Count > 1 || generatedEpisodes.Any(e => e.Season.HasValue || e.EpisodeNumber.HasValue))
                    series.SeriesFormat = SeriesFormat.Series;
                else
                    series.SeriesFormat = SeriesFormat.Single;

                context.SaveChanges();
            }


            // 3. Generate Wishlists by picking random User and Series
            var wishlistFaker = new Faker<Wishlist>()
                .RuleFor(x => x.UserId, f => f.PickRandom(users).Id)
                .RuleFor(x => x.SeriesId, f => f.PickRandom(seriesList).Id);

            var wishlists = wishlistFaker.Generate(20);
            context.Wishlists.AddRange(wishlists);
            context.SaveChanges();

            // Generate impression
            var impressionFaker = new Faker<Impression>()
                .RuleFor(x => x.IsRecommended, x => x.PickRandom(false, true))
                .RuleFor(x => x.Rating, x => x.Random.Int(1, 5))
                .RuleFor(x => x.SeriesId, f => f.PickRandom(seriesList).Id)
                .RuleFor(x => x.UserId, f => f.PickRandom(users).Id);

            var impressions = impressionFaker.Generate(20);
            context.Impressions.AddRange(impressions);
            context.SaveChanges();

            //Generate History unsure how - later

            //Generate Comment
            var CommentFaker = new Faker<Comment>()
                .RuleFor(x => x.Discussion, x => x.Lorem.Sentences())
                .RuleFor(x => x.UserId, f => f.PickRandom(users).Id)
                .RuleFor(e => e.CreatedAt, f => f.Date.Past())
                .RuleFor(x => x.EpisodeId, f => f.PickRandom(episodes).Id);

            var comments = CommentFaker.Generate(20);
            context.Comments.AddRange(comments);
            context.SaveChanges();


            // Generate Videos
            var videoFaker = new Faker<Video>()
                .RuleFor(v => v.VideoUrl, f => f.Internet.UrlWithPath())
                .RuleFor(v => v.Duration, f => f.Random.Int(60, 3600)) // duration in seconds
                                                                       // .RuleFor(v => v.Metadata, f => f.Lorem.Sentence())
                .RuleFor(v => v.CreatedAt, f => f.Date.Past())
                .RuleFor(v => v.UpdatedAt, f => f.Date.Recent())
                .RuleFor(v => v.ViewCount, f => f.Random.Int(0, 100000));

            var videos = new List<Video>();
            foreach (var ep in episodes)
            {
                int videoCount = new Random().Next(1, 2); // Assuming 1 video per episode
                var generatedVideos = videoFaker.Generate(videoCount);
                foreach (var v in generatedVideos)
                {
                    v.EpisodeId = ep.Id;
                }
                videos.AddRange(generatedVideos);
            }
            context.Videos.AddRange(videos);
            context.SaveChanges();

            // Generate Categories
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0]);

            var categories = categoryFaker.Generate(10);
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Generate Tags
            var tagFaker = new Faker<Tag>()
                .RuleFor(t => t.Name, f => f.Lorem.Word());

            var tags = tagFaker.Generate(10);
            context.Tags.AddRange(tags);
            context.SaveChanges();

            // SeriesCategory relationships
            var seriesCategories = new List<SeriesCategory>();
            foreach (var s in seriesList)
            {
                var randomCategories = categories.OrderBy(_ => Guid.NewGuid()).Take(new Random().Next(1, 3)).ToList();
                foreach (var c in randomCategories)
                {
                    seriesCategories.Add(new SeriesCategory
                    {
                        SeriesId = s.Id,
                        CategoryId = c.Id
                    });
                }
            }
            context.SeriesCategories.AddRange(seriesCategories);
            context.SaveChanges();

            // TagCategory relationships
            var tagCategories = new List<TagCategory>();
            foreach (var s in seriesList)
            {
                var randomTags = tags.OrderBy(_ => Guid.NewGuid()).Take(new Random().Next(1, 3)).ToList();
                foreach (var t in randomTags)
                {
                    tagCategories.Add(new TagCategory
                    {
                        SeriesId = s.Id,
                        TagId = t.Id
                    });
                }
            }
            context.TagCategories.AddRange(tagCategories);
            context.SaveChanges();

        }
    }

}