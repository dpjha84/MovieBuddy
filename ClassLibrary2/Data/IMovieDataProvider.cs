using System;
using System.Collections.Generic;

namespace MovieBuffLib
{
    public interface IMovieDataProvider
    {
        List<TmdbMovie> GetData(int year, int month);

        //CastDetail GetCast(int movieId);

        //string GetTrailer(int movieId);
    }

    public class Movie
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public string Trailor { get; set; }
        public List<Review> Reviews { get; set; }
    }

    public class Review
    {
        public string Reviewer { get; set; }
        public string Text { get; set; }
        public string Rating { get; set; }
        public int Image { get; set; }
    }

    public class DummyDataProvider// : IMovieDataProvider
    {
        //static Dictionary<string, string> ToiRatings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        //static Dictionary<string, string> ImdbRatings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        //static Dictionary<string, string> HtRatings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        //public async Task<List<Review>> GetReviews(int movieId, string movieName, string imdbId, bool upcoming)
        //{
        //    if (upcoming) return new List<Review>();

        //    //var t1 = MovieManager.GetToiRating(movieId, movieName);
        //    //var t2 = GetHtRating(movieId, movieName);
        //    //var t3 = GetImdbData(movieId, imdbId);
        //    //var result = await Task.WhenAll(t1, t2, t3);
        //    //var result = await Task.WhenAll(t1);
        //    return new List<Review>
        //    {
        //        new Review
        //        {
        //            Reviewer = "Times of India",
        //            Text = "Nice Movie",
        //            Rating = MovieManager.Instance.GetToiRating(movieId, movieName, true),
        //            Image = ""// Resource.Drawable.toi2
        //        },
        //        new Review
        //        {
        //            Reviewer = "Hindustan Times",
        //            Text = "Nice Movie",
        //            Rating = MovieManager.Instance.GetHtRating(movieId, movieName, true),
        //            Image = ""//Resource.Drawable.ht
        //        },
        //        new Review
        //        {
        //            Reviewer = "IMDB",
        //            Text = "Average Movie",
        //            Rating = MovieManager.Instance.GetImdbData(movieId, imdbId, true),
        //            Image = ""//Resource.Drawable.imdb
        //        }
        //    };
        //}

        //async static Task<string> GetImdbData(int movieId, string imdbId)
        //{
        //    try
        //    {
        //        if (imdbId == null) return null;
        //        var cached = Cache.GetImdbRating(movieId.ToString());
        //            if (cached != null) return cached;

        //        var url = $"http://www.imdb.com/title/{imdbId}";
        //        var web = new HtmlWeb();
        //        var doc = web.Load(url);
        //        var node = doc.GetElementbyId("title-overview-widget");
        //        var data = node.SelectNodes("div[2]/div[2]/div/div[1]/div[1]/div[1]/strong/span").First();
        //        if (data == null || string.IsNullOrWhiteSpace(data.InnerText))
        //            return null;
        //        var rating = $"{data.InnerText}/10";
        //        Cache.AddToImdbRating(movieId.ToString(), rating);
        //        return rating;
        //    }
        //    catch(Exception)
        //    {
        //        return null;
        //    }
        //}

        //async static Task<string> GetToiRating(int movieId, string movieName)
        //{
        //    try
        //    {
        //        //if (ToiRatings.ContainsKey(movieName))
        //        var cached = Cache.GetToiRating(movieId.ToString());
        //        if (cached != null) return cached;

        //        var movieName1 = movieName.Replace(" ", "+");
        //        var movieName2 = movieName.Replace(" ", "-");
        //        var url1 = $"https://www.google.co.in/search?q={movieName1}+review";
        //        string ss = $"https://timesofindia.indiatimes.com/entertainment/hindi/movie-reviews/{movieName2}/movie-review/";
        //        using (var client = new WebClient())
        //        {
        //            var datastream = client.OpenRead(url1);
        //            using (StreamReader reader = new StreamReader(datastream))
        //            {
        //                StringBuilder sb = new StringBuilder();
        //                while (!reader.EndOfStream)
        //                    sb.Append(reader.ReadLine());
        //                var data = sb.ToString();
        //                var index = data.IndexOf(ss, StringComparison.OrdinalIgnoreCase);
        //                data = data.Substring(index, ss.Length + 12);
        //                var web = new HtmlWeb();
        //                var doc = web.Load(data);
        //                var node = doc.GetElementbyId("content");
        //                var data1 = node.SelectNodes("div/div[3]/div[1]/div[2]/div[1]/span[3]").First();
        //                var rating = data1.InnerText;
        //                if (!string.IsNullOrWhiteSpace(rating))
        //                    //ToiRatings.Add(movieName, rating);
        //                    Cache.AddToToiRating(movieId.ToString(), rating);
        //                return rating;
        //            }
        //        }
        //    }
        //    catch(Exception)
        //    {
        //        return null;
        //    }
        //}

        //async static Task<string> GetHtRating(int movieId, string movieName)
        //{
        //    try
        //    {
        //        var cached = Cache.GetHtRating(movieId.ToString());
        //        if (cached != null) return cached;

        //        var movieName1 = movieName.Replace(" ", "+");
        //        var movieName2 = movieName.Replace(" ", "-");
        //        var url1 = $"https://www.google.co.in/search?q={movieName1}+review";
        //        string ss = $"http://www.hindustantimes.com/movie-reviews/{movieName2}-movie-review-";
        //        using (var client = new WebClient())
        //        {
        //            var datastream = client.OpenRead(url1);
        //            using (StreamReader reader = new StreamReader(datastream))
        //            {
        //                StringBuilder sb = new StringBuilder();
        //                while (!reader.EndOfStream)
        //                    sb.Append(reader.ReadLine());
        //                var data = sb.ToString();
        //                var index = data.IndexOf(ss, StringComparison.OrdinalIgnoreCase);
        //                var lastIndex = data.IndexOf(".html", index);
        //                data = data.Substring(index, lastIndex - index + 5);
        //                var web = new HtmlWeb();
        //                var doc = web.Load(data);
        //                HtmlNode data1;
        //                try
        //                {
        //                    data1 = doc.DocumentNode.SelectNodes("/html/body/div[1]/section/div[1]/div/div[1]/article/div[2]/div[2]/p[1]/text()[3]").First();
        //                }
        //                catch (Exception)
        //                {
        //                    data1 = doc.DocumentNode.SelectNodes("/html/body/div[1]/section/div/div/div[1]/article/div[2]/div[2]/p[2]/text()[3]").First();
        //                }
        //                var rating = data1.InnerText;
        //                if (!string.IsNullOrWhiteSpace(rating))
        //                    //HtRatings.Add(movieName, rating);
        //                    Cache.AddToHtRating(movieId.ToString(), rating);
        //                return rating;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public List<SearchMovie> GetData(int year, int month)
        //{
        //    //TMDbLib.Client.TMDbClient client = new TMDbLib.Client.TMDbClient("");
        //    //var data = client.SearchMovieAsync("Ribbon");
        //    var currentTime = DateTime.Now;
        //    var CurrentDay = currentTime.Day;

        //    var CurrentYear = currentTime.Year;
        //    var CurrentMonth = currentTime.Month;
        //    var PreviousMonth = currentTime.AddMonths(-1).Month;
        //    var PreviousYear = currentTime.AddMonths(-1).Year;
        //    var NextMonth = currentTime.AddMonths(1).Month;
        //    var NextYear = currentTime.AddMonths(1).Year;
        //    return new List<SearchMovie>();
        //    //if (year == PreviousYear && month == PreviousMonth)
        //    //    return new List<SearchMovie>
        //    //    {
        //    //        new SearchMovie
        //    //        {
        //    //            Title = "Don",
        //    //            PosterPath = "http://i.imgur.com/DvpvklR.png",
        //    //            ReleaseDate = currentTime.AddDays(-1),
        //    //            Summary = "Summary Don",
        //    //            Trailor = "https://www.youtube.com/watch?v=X_5_BLt76c0",
        //    //            Reviews = GetReviews()
        //    //        }
        //    //    };

        //    //else if (year == CurrentYear && month == CurrentMonth)
        //    //    return new List<SearchMovie>
        //    //    {
        //    //        new Movie
        //    //        {
        //    //            Title = "Intersteller",
        //    //            ImageUrl = "http://i.imgur.com/DvpvklR.png",
        //    //            Date = currentTime,
        //    //            Summary = "Summary Intersteller",
        //    //            Trailor = "https://www.youtube.com/watch?v=X_5_BLt76c0",
        //    //            Reviews = GetReviews()
        //    //        }
        //    //    };

        //    //else
        //    //    return new List<SearchMovie>
        //    //    {
        //    //        new Movie
        //    //        {
        //    //            Title = "Inception",
        //    //            ImageUrl = "http://i.imgur.com/DvpvklR.png",
        //    //            Date = currentTime.AddDays(1),
        //    //            Summary = "Summary Inception",
        //    //            Trailor = "https://www.youtube.com/watch?v=X_5_BLt76c0",
        //    //            Reviews = GetReviews()
        //    //        }
        //    //    };
        //}

        //public CastDetail GetCast(int movieId)
        //{
        //   return new CastDetail { Cast = new List<Cast> { new Cast { Name = "Aamir Khan"} } };
        //}

        //public string GetTrailer(int movieId)
        //{
        //    return null;
        //}
    }
}