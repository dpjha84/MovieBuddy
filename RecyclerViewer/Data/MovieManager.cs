using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMDbLib.Objects.Search;
using RecyclerViewer.Data;
using Android.Widget;
using Android.Support.V7.App;
using Com.Bumptech.Glide;
using Square.Picasso;
using Android.Content;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using System.IO;
using System.Text;

namespace RecyclerViewer
{    
    public class Helper
    {
        internal static void SetImage(Context castInfoActivity, int resourceId, ImageView imageView)
        {
            Glide.With(castInfoActivity).Load(resourceId).Into(imageView);
        }

        internal static void SetImage(Context castInfoActivity, Java.Lang.Object resourceId, ImageView imageView)
        {
            Glide.With(castInfoActivity).Load(resourceId).Into(imageView);
        }

        internal static void SetImage(Context castInfoActivity, string backdrop, ImageView imageView)
        {
            Glide.With(castInfoActivity).Load(backdrop).Into(imageView);
        }
    }
    public class MovieManager
    {
        public static IMovieDataProvider dataProvider;

        private static List<SearchMovie> _previousMonthMovies = new List<SearchMovie>();
        public static List<SearchMovie> PreviousMonthMovies
        {
            get { return _previousMonthMovies; }
            set
            {
                _previousMonthMovies = value;
                var data = JsonConvert.SerializeObject(value);
                LocalDataProvider.Set($"{PreviousYear}{PreviousMonth}", data);
            }
        }

        private static List<SearchMovie> _nextMonthMovies = new List<SearchMovie>();
        public static List<SearchMovie> NextMonthMovies
        {
            get { return _nextMonthMovies; }
            set
            {
                _nextMonthMovies = value;
                LocalDataProvider.Set($"{NextYear}{NextMonth}", JsonConvert.SerializeObject(value));
            }
        }

        private static List<SearchMovie> _currentMonthMovies = new List<SearchMovie>();
        public static List<SearchMovie> CurrentMonthMovies
        {
            get { return _currentMonthMovies; }
            set
            {
                _currentMonthMovies = value;
                LocalDataProvider.Set($"{CurrentYear}{CurrentMonth}", JsonConvert.SerializeObject(value));
            }
        }

        public static int PreviousMonth { get; set; }
        public static int CurrentMonth { get; set; }
        public static int NextMonth { get; set; }
        public static int PreviousYear { get; set; }
        public static int CurrentYear { get; set; }
        public static int NextYear { get; set; }
        public static int CurrentDay { get; set; }

        public static List<SearchMovie> GetReleased()
        {
            var list = _previousMonthMovies;
            var newList = list.ToList();
            newList.AddRange(_currentMonthMovies.Where(c => c.ReleaseDate?.Day <= CurrentDay));
            return newList.OrderByDescending(m => m.ReleaseDate).ToList();
        }

        public static List<SearchMovie> GetUpcoming()
        {
            var list = _currentMonthMovies.Where(c => c.ReleaseDate?.Day > CurrentDay).ToList();
            list.AddRange(_nextMonthMovies);
            return list.OrderBy(m => m.ReleaseDate).ToList();
        }

        public static SearchMovie FindMovie(string movieName)
        {
            var movie = PreviousMonthMovies.ToList().Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            if (movie == null)
            {
                movie = CurrentMonthMovies.ToList().Where(m => m.OriginalTitle == movieName).FirstOrDefault();
                if (movie == null)
                    movie = NextMonthMovies.ToList().Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            }
            return movie;
        }

        public static bool IsMoviePresent(string movieName)
        {
            var movie = PreviousMonthMovies.ToList().Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            if (movie == null)
            {
                movie = CurrentMonthMovies.ToList().Where(m => m.OriginalTitle == movieName).FirstOrDefault();
                if (movie == null)
                    movie = NextMonthMovies.ToList().Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            }
            return movie != null;
        }        

        public static CastDetail GetCast(int movieId, bool allowCachedData = true)
        {
            if (movieId == 0) return new CastDetail { Cast = new List<Cast>()};

            if (allowCachedData)
            {
                var castDetail = Cache.GetCast(movieId);
                if (castDetail != null) return castDetail;
            }

            var data = new TMdbDataProvider().GetCast(movieId);
            Cache.AddToCast(movieId, data);
            return data;
        }

        public static CastDetail GetMovieCredits(long castId)
        {
            return new TMdbDataProvider().GetMovieCredits(castId);
        }

        public static TmdbMovie GetMovie(int movieId, bool allowCachedData = true)
        {
            if (movieId == 0) return null;
            if (allowCachedData)
            {
                var data1 = Cache.GetMovie(movieId);
                if (data1 != null) return data1;
            }

            var data = new TMdbDataProvider().GetMovie(movieId);
            Cache.AddToMovie(movieId, data);
            return data;
        }

        public static string GetTrailer(int movieId, bool allowCachedData = true)
        {
            if (movieId == 0) return null;
            if (allowCachedData)
            {
                var data = Cache.GetTrailer(movieId);
                if (data != null) return data;
            }            

            var dat = new TMdbDataProvider().GetTrailer(movieId);
            Cache.AddToTrailer(movieId, dat);
            return dat;
        }

        public static string GetImdbData(int movieId, string imdbId, bool allowCachedData = true)
        {
            try
            {
                if (imdbId == null) return null;
                if (allowCachedData)
                {
                    var cached = Cache.GetImdbRating(movieId.ToString());
                    if (cached != null) return cached;
                }

                var url = $"http://www.imdb.com/title/{imdbId}";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var node = doc.GetElementbyId("title-overview-widget");
                var data = node.SelectNodes("div[2]/div[2]/div/div[1]/div[1]/div[1]/strong/span").First();
                if (data == null || string.IsNullOrWhiteSpace(data.InnerText))
                    return null;
                var rating = $"{data.InnerText}/10";
                Cache.AddToImdbRating(movieId.ToString(), rating);
                return rating;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetToiRating(int movieId, string movieName, bool allowCachedData = true)
        {
            try
            {
                string movieKey = movieId.ToString();
                if (movieId == 0)
                    movieKey = movieName.Replace(" ", "");
                if (allowCachedData)
                {
                    var cached = Cache.GetToiRating(movieKey);
                    if (cached != null) return cached;
                }

                var movieName1 = movieName.Replace(" ", "+");
                var movieName2 = movieName.Replace(" ", "-");
                var url1 = $"https://www.google.co.in/search?q={movieName1}+review";
                string ss = $"https://timesofindia.indiatimes.com/entertainment/hindi/movie-reviews/{movieName2}/movie-review/";
                using (var client = new WebClient())
                {
                    var datastream = client.OpenRead(url1);
                    using (StreamReader reader = new StreamReader(datastream))
                    {
                        StringBuilder sb = new StringBuilder();
                        while (!reader.EndOfStream)
                            sb.Append(reader.ReadLine());
                        var data = sb.ToString();
                        var index = data.IndexOf(ss, StringComparison.OrdinalIgnoreCase);
                        data = data.Substring(index, ss.Length + 12);
                        var web = new HtmlWeb();
                        var doc = web.Load(data);
                        var node = doc.GetElementbyId("content");
                        var data1 = node.SelectNodes("div/div[3]/div[1]/div[2]/div[1]/span[3]").First();
                        var rating = data1.InnerText;
                        if (!string.IsNullOrWhiteSpace(rating))
                            //ToiRatings.Add(movieName, rating);
                            Cache.AddToToiRating(movieKey, rating);
                        return rating;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetHtRating(int movieId, string movieName, bool allowCachedData = true)
        {
            try
            {
                string movieKey = movieId.ToString();
                if (movieId == 0)
                    movieKey = movieName.Replace(" ", "");
                if (allowCachedData)
                {
                    var cached = Cache.GetHtRating(movieKey);
                    if (cached != null) return cached;
                }

                var movieName1 = movieName.Replace(" ", "+");
                var movieName2 = movieName.Replace(" ", "-");
                var url1 = $"https://www.google.co.in/search?q={movieName1}+review";
                string ss = $"http://www.hindustantimes.com/movie-reviews/{movieName2}-movie-review-";
                using (var client = new WebClient())
                {
                    var datastream = client.OpenRead(url1);
                    using (StreamReader reader = new StreamReader(datastream))
                    {
                        StringBuilder sb = new StringBuilder();
                        while (!reader.EndOfStream)
                            sb.Append(reader.ReadLine());
                        var data = sb.ToString();
                        var index = data.IndexOf(ss, StringComparison.OrdinalIgnoreCase);
                        var lastIndex = data.IndexOf(".html", index);
                        data = data.Substring(index, lastIndex - index + 5);
                        var web = new HtmlWeb();
                        var doc = web.Load(data);
                        HtmlNode data1;
                        try
                        {
                            data1 = doc.DocumentNode.SelectNodes("/html/body/div[1]/section/div[1]/div/div[1]/article/div[2]/div[2]/p[1]/text()[3]").First();
                        }
                        catch (Exception)
                        {
                            data1 = doc.DocumentNode.SelectNodes("/html/body/div[1]/section/div/div/div[1]/article/div[2]/div[2]/p[2]/text()[3]").First();
                        }
                        var rating = data1.InnerText;
                        if (!string.IsNullOrWhiteSpace(rating))
                            //HtRatings.Add(movieName, rating);
                            Cache.AddToHtRating(movieKey, rating);
                        return rating;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        static MovieManager()
        {
            LoadInternalAsync();
        }

        private static async void LoadInternalAsync()
        {
            try
            {
                //LocalDataProvider.Reset();
                dataProvider = new BollywoodMdbDataProvider();
                //if (dataProvider == null) return;

                var currentTime = DateTime.Now;
                CurrentDay = currentTime.Day;

                CurrentYear = currentTime.Year;
                CurrentMonth = currentTime.Month;
                PreviousMonth = currentTime.AddMonths(-1).Month;
                PreviousYear = currentTime.AddMonths(-1).Year;
                NextMonth = currentTime.AddMonths(1).Month;
                NextYear = currentTime.AddMonths(1).Year;

                PreviousMonthMovies = GetData(dataProvider, PreviousYear, PreviousMonth);
                foreach (var movie in PreviousMonthMovies)
                    ReadMovieDataFromDisk(movie);

                CurrentMonthMovies = GetData(dataProvider, CurrentYear, CurrentMonth);
                foreach (var movie in CurrentMonthMovies)
                    ReadMovieDataFromDisk(movie);

                NextMonthMovies = GetData(dataProvider, NextYear, NextMonth);
                foreach (var movie in NextMonthMovies)
                    ReadMovieDataFromDisk(movie);
            }
            catch (Exception ex)
            {

            }
        }

        private static void ReadMovieDataFromDisk(SearchMovie movie)
        {
            TmdbMovie tmdbMovie;
            var data = LocalDataProvider.Get($"{movie.Id}m");
            if (!string.IsNullOrWhiteSpace(data))
            {
                tmdbMovie = TmdbMovie.FromJson(data);
                Cache.AddToMovie(movie.Id, tmdbMovie, true);
            }
            else
                tmdbMovie = GetMovie(movie.Id, false);

            data = LocalDataProvider.Get($"{movie.Id}t");
            if (!string.IsNullOrWhiteSpace(data))
                Cache.AddToTrailer(movie.Id, data, true);
            else
                GetTrailer(movie.Id, false);

            if (movie.ReleaseDate <= DateTime.Now)
            {
                data = LocalDataProvider.Get($"{movie.Id}tr");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    string movieKey = movie.Id.ToString();
                    if (movie.Id == 0)
                        movieKey = movie.Title.Replace(" ", "");
                    Cache.AddToToiRating(movieKey, data, true);
                }
                else
                    GetToiRating(movie.Id, movie.Title, false);

                data = LocalDataProvider.Get($"{movie.Id}hr");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    string movieKey = movie.Id.ToString();
                    if (movie.Id == 0)
                        movieKey = movie.Title.Replace(" ", "");
                    Cache.AddToHtRating(movieKey, data, true);
                }
                else
                    GetHtRating(movie.Id, movie.Title, false);

                data = LocalDataProvider.Get($"{movie.Id}ir");
                if (!string.IsNullOrWhiteSpace(data))
                    Cache.AddToImdbRating(movie.Id.ToString(), data, true);
                else
                    GetImdbData(movie.Id, tmdbMovie?.ImdbId, false);
            }

            data = LocalDataProvider.Get($"{movie.Id}c");
            if (!string.IsNullOrWhiteSpace(data))
                Cache.AddToCast(movie.Id, CastDetail.FromJson(data), true);
            else
                GetCast(movie.Id, false);
        }

        public static void Load()
        {
            LoadInternalAsync();
        }

        private static List<SearchMovie> GetData(IMovieDataProvider dataProvider, int year, int month)
        {
            var data = LocalDataProvider.Get($"{year}{month}");
            if (!string.IsNullOrWhiteSpace(data))
                return JsonConvert.DeserializeObject<List<SearchMovie>>(data);
            else
                return dataProvider.GetData(year, month);
        }
    }    
}