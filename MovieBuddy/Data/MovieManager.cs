﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;
using TCast = TMDbLib.Objects.Movies.Cast;
using TReview = TMDbLib.Objects.Reviews.ReviewBase;
using MovieBuddy.Data;

namespace MovieBuddy
{
    public class MovieManager
    {
        readonly Dictionary<int, string> genreMap = new Dictionary<int, string>();
        readonly Dictionary<string, string> languageMap = new Dictionary<string, string>();
        readonly TMDbClient tmdbClient;
        const string tmdbApiKey = "c6b31d1cdad6a56a23f0c913e2482a31";
        int personMoviesTotal = 0;
        MovieCredits movieCredits;
        int totalCast = 0;
        TMDbLib.Objects.Movies.Credits Credits;
        string nowPlayingBaseUrl, upcomingBaseUrl;
        Func<TSMovie, bool> filterNowPlaying, filterUpcoming;
        int nowPlayingTotalPage = int.MaxValue, upcomingTotalPage = int.MaxValue, popularTotalPage = int.MaxValue, topRatedTotalPage = int.MaxValue;

        public static MovieManager Instance { get; private set; }
        static bool loaded = false;

        public static void Init()
        {
            if (Instance == null && !loaded)
            {
                Instance = new MovieManager();
                Instance.Load();
                loaded = true;
            }
        }

        public static void EnsureLoaded()
        {
            Init();
        }

        private MovieManager()
        {
            tmdbClient = new TMDbClient(tmdbApiKey);
        }

        T GetCachedOrWebData<T>(string key, Func<T> getDataFromWeb = null)
        {
            string cacheData = LocalCache.Instance.Get(key);
            if (string.IsNullOrWhiteSpace(cacheData) && getDataFromWeb != null)
            {
                T webData = getDataFromWeb();
                LocalCache.Instance.Set(key, JsonConvert.SerializeObject(webData));
                return webData;
            }
            return JsonConvert.DeserializeObject<T>(cacheData);
        }

        void Load()
        {
            foreach (var g in GetCachedOrWebData(LocalCache.GenresKey, () => tmdbClient.GetMovieGenresAsync().Result))
                genreMap.Add(g.Id, g.Name);

            foreach (var l in GetCachedOrWebData(LocalCache.LanguagesKey, () => tmdbClient.GetLanguagesAsync().Result))
                languageMap.Add(l.Iso_639_1, l.EnglishName);

            var today = DateTime.Now;
            var endDate = today.ToString("yyyy-MM-dd");
            var startDate = today.Subtract(TimeSpan.FromDays(60)).ToString("yyyy-MM-dd");
            nowPlayingBaseUrl = BuildUrl(startDate, "release_date.desc", endDate);
            upcomingBaseUrl = BuildUrl(today.AddDays(1).ToString("yyyy-MM-dd"), "release_date.asc");
            filterNowPlaying = x => x.ReleaseDate >= DateTime.Parse(startDate) && x.ReleaseDate <= DateTime.Parse(endDate);
            filterUpcoming = x => x.ReleaseDate > DateTime.Parse(endDate);
        }

        public List<TSMovie> SearchMovie(string query, int page)
        {
            return tmdbClient.SearchMovieAsync(query, page).Result.Results;
        }

        public List<TMDbLib.Objects.Search.SearchPerson> SearchPerson(string query, int page)
        {
            return tmdbClient.SearchPersonAsync(query, page).Result.Results;
        }

        public List<TSMovie> GetUpcoming(int page)
        {
            if (page == 1) upcomingTotalPage = int.MaxValue;
            if (page > upcomingTotalPage) return null;
            if(Globals.SelectedLanguage == "All") return tmdbClient.GetMovieUpcomingListAsync(null, page).Result.Results;

            var result = tmdbClient.GetMoviesByUrl(upcomingBaseUrl, page);
            upcomingTotalPage = result.TotalPages;
            return result.Results.Where(filterUpcoming).ToList();
        }        

        string BuildUrl(string startDate, string sortBy, string endDate = null)
        {
            var sb = new StringBuilder();
            sb.Append("https://api.themoviedb.org/3/discover/movie?");
            sb.Append($"api_key={tmdbApiKey}&");
            sb.Append($"release_date.gte={startDate}&");
            sb.Append($"sort_by={sortBy}&");
            if (endDate != null)
                sb.Append($"release_date.lte={endDate}&");            
            return sb.ToString();
        }
        
        public List<TSMovie> GetNowPlaying(int page)
        {
            if (page == 1) nowPlayingTotalPage = int.MaxValue;
            if (page > nowPlayingTotalPage) return null;
            if (Globals.SelectedLanguage == "All") return tmdbClient.GetMovieNowPlayingListAsync(null, page).Result.Results;

            var result = tmdbClient.GetMoviesByUrl(nowPlayingBaseUrl, page);
            nowPlayingTotalPage = result.TotalPages;
            return result.Results.Where(filterNowPlaying).ToList();
        }

        public List<TSMovie> GetPopular(int page)
        {
            if (page > popularTotalPage) return null;
            var res = tmdbClient.GetMoviePopularListAsync(null, page).Result;
            popularTotalPage = res.TotalPages;
            return res.Results;
        }

        public List<TSMovie> GetTopRated(int page)
        {
            if (page > topRatedTotalPage) return null;
            var res = tmdbClient.GetMovieTopRatedListAsync(null, page).Result;
            topRatedTotalPage = res.TotalPages;
            return res.Results;
        }

        public string GetTrailer(int movieId)
        {
            var videos = tmdbClient.GetMovieVideosAsync(movieId).Result.Results;
            if (videos == null || videos.Count == 0) return null;
            return videos.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Key)).Key;
        }

        public Person GetPerson(int castId)
        {
            var data = tmdbClient.GetPersonAsync(castId).Result;
            return data;
        }
        
        public List<TCast> GetCastAndCrew(int movieId, int page = 1)
        {
            if (page == 1)
            {
                Credits = tmdbClient.GetMovieCreditsAsync(movieId).Result;
                totalCast = Credits.Cast.Count;
            }
            if (page > totalCast / 15 + 1) return null;
            return Credits.Cast.Skip((page - 1) * 15).Take(15).ToList();
        }

        public List<TSMovie> GetSimilar(int movieId, int page)
        {
            return tmdbClient.GetMovieSimilarAsync(movieId, page).Result.Results;
        }

        public List<TReview> GetReviews(int movieId) => tmdbClient.GetMovieReviewsAsync(movieId).Result.Results;
        
        public List<MovieRole> GetMovieCredits(int personId, int page)
        {
            if (page == 1)
            {
                movieCredits = tmdbClient.GetPersonMovieCreditsAsync(personId).Result;
                personMoviesTotal = movieCredits.Cast.Count;
            }
            if (page > personMoviesTotal / 15 + 1) return null;
            return movieCredits.Cast.Skip((page - 1) * 15).Take(15).ToList();
        }

        public Dictionary<string, string> GetFullOverview(int movieId)
        {
            Dictionary<string, string> summaryMap = new Dictionary<string, string>();
            var movie = tmdbClient.GetMovieAsync(movieId).Result;
            summaryMap.Add("Overview", movie.Overview);
            if (movie.ReleaseDate.HasValue)
                summaryMap.Add("Year", movie.ReleaseDate.Value.Year.ToString());
            if (movie.Genres != null)
                summaryMap.Add("Genres", string.Join(", ", movie.Genres.Select(x => genreMap[x.Id])));
            if (movie.ReleaseDate.HasValue)
                summaryMap.Add("Release date", movie.ReleaseDate.Value.ToString("dd MMMM yyyy"));
            summaryMap.Add("Language", languageMap[movie.OriginalLanguage]);
            return summaryMap;
        }

        public string GetGenreText(int genreId)
        {
            return genreMap[genreId];
        }
    }
}
