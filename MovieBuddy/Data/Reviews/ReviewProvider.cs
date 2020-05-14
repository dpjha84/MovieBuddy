using HtmlAgilityPack;
using System;

namespace MovieBuddy
{
    public abstract class ReviewProvider : IReviewProvider
    {
        public ReviewProvider()
        {
        }

        public abstract string GetUrl(string movieName);

        public abstract HtmlNode GetRatingNode(string url);

        public abstract string GetRating(HtmlNode doc);

        //public async Task<string> GetImdbData(string key, int movieId, string imdbId)
        //{
        //    var sw = Stopwatch.StartNew();
        //    try
        //    {
        //        if (imdbId == null) return null;

        //        var url = $"http://www.imdb.com/title/{imdbId}";
        //        var web = new HtmlWeb();
        //        var doc = web.Load(url);
        //        var node = doc.GetElementbyId("title-overview-widget");
        //        var data = node.SelectNodes("div[2]/div[2]/div/div[1]/div[1]/div[1]/strong/span").First();
        //        if (data == null || string.IsNullOrWhiteSpace(data.InnerText))
        //            return null;
        //        var rating = $"{data.InnerText}/10";
        //        Console.WriteLine($"IMDB - {movieId} " + sw.Elapsed);
        //        return rating;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public async Task<string> GetToiRating(string key, int movieId, string movieName)
        //{
        //    var sw = Stopwatch.StartNew();
        //    try
        //    {
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
        //                //var node = doc.GetElementbyId("mainwrapper");
        //                //var data1 = node.SelectNodes("div[2]/div[2]/div[8]/div[2]/span[2]/span").First();
        //                var node = doc.GetElementbyId("dynamiccontent");
        //                var data1 = node.SelectNodes("div/span[2]/span[2]").First();
        //                var rating = data1.InnerText;
        //                Console.WriteLine($"TOI - {movieName} " + sw.Elapsed);
        //                if (!string.IsNullOrWhiteSpace(rating))
        //                    rating += "/5";
        //                //ToiRatings.Add(movieName, rating);
        //                //Cache.AddToToiRating(movieKey, rating);
        //                MovieData[key].ToiRating = rating;
        //                return rating;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public async Task<string> GetHtRating(string key, int movieId, string movieName)
        //{
        //    var sw = Stopwatch.StartNew();
        //    try
        //    {
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
        //                Console.WriteLine($"HT - {movieName} " + sw.Elapsed);
        //                var rating = data1.InnerText;
        //                MovieData[key].HtRating = rating;
        //                return rating;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        public virtual string GetData(string key)
        {
            //return null;
            try
            {
                if (key == null) return null;
                var url = GetUrl(key);
                var node = GetRatingNode(url);
                return GetRating(node);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}