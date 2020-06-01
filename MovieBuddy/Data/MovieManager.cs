using Newtonsoft.Json;
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

namespace MovieBuddy
{
    public class MovieManager
    {
        Dictionary<int, string> genreMap = new Dictionary<int, string>();
        Dictionary<string, string> languageMap = new Dictionary<string, string>();
        readonly TMDbClient tmdbClient;
        private List<TMDbLib.Objects.Search.SearchMovie> upcoming;
        private List<TMDbLib.Objects.Search.SearchMovie> nowPlaying;
        private List<TMDbLib.Objects.Search.SearchMovie> popular;
        private List<TMDbLib.Objects.Search.SearchMovie> topRated;
        public static MovieManager Instance { get; private set; }

        public static void Init()
        {
            if (Instance == null)
            {
                Instance = new MovieManager();
                Instance.Load();
            }
        }

        private MovieManager()
        {
            tmdbClient = new TMDbClient("c6b31d1cdad6a56a23f0c913e2482a31");
        }

        void Load()
        {
            var data = LocalDataProvider.Instance.Get("GenreList");
            List<TMDbLib.Objects.General.Genre> list1;
            if (data == null)
            {
                list1 = tmdbClient.GetMovieGenresAsync().Result;
                LocalDataProvider.Instance.Set("GenreList", JsonConvert.SerializeObject(list1));
            }
            else
                list1 = JsonConvert.DeserializeObject<List<TMDbLib.Objects.General.Genre>>(data);

            foreach (var g in list1)
                genreMap.Add(g.Id, g.Name);

            List<TMDbLib.Objects.Languages.Language> list2;
            data = LocalDataProvider.Instance.Get("LanguageList");
            if (data == null)
            {
                list2 = tmdbClient.GetLanguagesAsync().Result;
                LocalDataProvider.Instance.Set("LanguageList", JsonConvert.SerializeObject(list2));
            }
            else
                list2 = JsonConvert.DeserializeObject<List<TMDbLib.Objects.Languages.Language>>(data);
            foreach (var l in list2)
                languageMap.Add(l.Iso_639_1, l.EnglishName);
        }

        public List<TMDbLib.Objects.Search.SearchMovie> GetUpcoming(int page)
        {
            if (page == 1) upcomingTotalPage = int.MaxValue;
            if (page > upcomingTotalPage) return null;
            if(Globals.SelectedLanguage == "All") return tmdbClient.GetMovieUpcomingListAsync(null, page).Result.Results;

            var startDate = DateTime.Now.ToString("yyyy-MM-dd");
            var lang = Globals.LanguageMap[Globals.SelectedLanguage];
            var requestUrl = "https://api.themoviedb.org/3/discover/movie?" +
                "api_key=c6b31d1cdad6a56a23f0c913e2482a31&" +
                $"release_date.gte={startDate}&" +
                "sort_by=release_date.asc&" +
                "include_adult=false&" +
                "include_video=false&" +
                $"page={page}&" +
                $"with_original_language={lang}";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(requestUrl);
                if (response.Result.IsSuccessStatusCode)
                {
                    var data = response.Result.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<SearchContainer<TMDbLib.Objects.Search.SearchMovie>>(data.Result);
                    upcomingTotalPage = res.TotalPages;
                    return res.Results.Where(x => x.ReleaseDate >= DateTime.Parse(startDate)).ToList();
                }
            }
            return null;
        }
        private int nowPlayingTotalPage = int.MaxValue, upcomingTotalPage = int.MaxValue, popularTotalPage = int.MaxValue, topRatedTotalPage = int.MaxValue;
        public List<TMDbLib.Objects.Search.SearchMovie> GetNowPlaying(int page)
        {
            if (page == 1) nowPlayingTotalPage = int.MaxValue;
            if (page > nowPlayingTotalPage) return null;
            if (Globals.SelectedLanguage == "All") return tmdbClient.GetMovieNowPlayingListAsync(null, page).Result.Results;

            var endDate = DateTime.Now.ToString("yyyy-MM-dd");
            var startDate = DateTime.Now.Subtract(TimeSpan.FromDays(60)).ToString("yyyy-MM-dd");
            var lang = Globals.LanguageMap[Globals.SelectedLanguage];
            var requestUrl = "https://api.themoviedb.org/3/discover/movie?" +
                "api_key=c6b31d1cdad6a56a23f0c913e2482a31&" +
                $"release_date.gte={startDate}&" +
                $"release_date.lte={endDate}&" +
                "sort_by=release_date.desc&" +
                "include_adult=false&" +
                "include_video=false&" +
                $"page={page}&" +
                $"with_original_language={lang}";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(requestUrl);
                if (response.Result.IsSuccessStatusCode)
                {
                    var data = response.Result.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<SearchContainer<TMDbLib.Objects.Search.SearchMovie>>(data.Result);
                    nowPlayingTotalPage = res.TotalPages;
                    return res.Results.Where(x => x.ReleaseDate >= DateTime.Parse(startDate) && x.ReleaseDate <= DateTime.Parse(endDate)).ToList();
                }
            }
            return null;
        }

        public List<TMDbLib.Objects.Search.SearchMovie> Upcoming
        {
            get
            {
                if (upcoming == null)
                {
                    upcoming = GetUpcoming(1);
                }
                // upcoming = tmdbClient.GetMovieUpcomingListAsync().Result.Results;
                return upcoming;
            }
        }
        public List<TMDbLib.Objects.Search.SearchMovie> NowPlaying
        {
            get
            {
                if (nowPlaying == null)
                {
                    nowPlaying = GetNowPlaying(1);
                }
                // nowPlaying = tmdbClient.GetMovieNowPlayingListAsync().Result.Results;
                return nowPlaying;
            }
        }
        public List<TMDbLib.Objects.Search.SearchMovie> TopRated
        {
            get
            {
                if (topRated == null)
                    topRated = tmdbClient.GetMovieTopRatedListAsync().Result.Results;
                return topRated;
            }
        }
        public List<TMDbLib.Objects.Search.SearchMovie> Popular
        {
            get
            {
                if (popular == null)
                    popular = tmdbClient.GetMoviePopularListAsync().Result.Results;
                return popular;
            }
        }

        public List<TMDbLib.Objects.Search.SearchMovie> GetPopular(int page)
        {
            if (page > popularTotalPage) return null;
            var res = tmdbClient.GetMoviePopularListAsync(null, page).Result;
            popularTotalPage = res.TotalPages;
            return res.Results;
        }

        public List<TMDbLib.Objects.Search.SearchMovie> GetTopRated(int page)
        {
            if (page > topRatedTotalPage) return null;
            var res = tmdbClient.GetMovieTopRatedListAsync(null, page).Result;
            topRatedTotalPage = res.TotalPages;
            return res.Results;
        }

        public string GetTrailer(int movieId, string name)
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

        int totalCast = 0;
        TMDbLib.Objects.Movies.Credits Credits;
        public List<TMDbLib.Objects.Movies.Cast> GetCastAndCrew(int movieId, int page = 1)
        {
            if (page == 1)
            {
                Credits = tmdbClient.GetMovieCreditsAsync(movieId).Result;
                totalCast = Credits.Cast.Count;
            }
            if (page > totalCast / 15 + 1) return null;
            return Credits.Cast.Skip((page - 1) * 15).Take(15).ToList();
        }

        int similarTotalPages = int.MaxValue;
        public List<TMDbLib.Objects.Search.SearchMovie> GetSimilar(int movieId, int page)
        {
            return tmdbClient.GetMovieSimilarAsync(movieId, page).Result.Results;
            //if (page > similarTotalPages) return null;
            //var res = tmdbClient.GetMovieSimilarAsync(movieId, page).Result;
            //similarTotalPages = res.TotalPages;
            //return res.Results;
        }

        public List<TMDbLib.Objects.Reviews.ReviewBase> GetReviews(int movieId) => tmdbClient.GetMovieReviewsAsync(movieId).Result.Results;

        int personMoviesTotal = 0;
        MovieCredits movieCredits;
        public List<MovieRole> GetMovieCredits(int personId, int page)
        {
            if (page == 1)
            {
                movieCredits = tmdbClient.GetPersonMovieCreditsAsync(personId).Result;
                personMoviesTotal = movieCredits.Cast.Count;
            }
            if (page > personMoviesTotal / 15 + 1) return null;
            return movieCredits.Cast.Skip((page - 1) * 15).Take(15).ToList();
            //return tmdbClient.GetPersonMovieCreditsAsync(personId).Result;
        }

        public Dictionary<string, string> GetFullOverview(int movieId)
        {
            Dictionary<string, string> summaryMap = new Dictionary<string, string>();
            var movie = Upcoming.Find(x => x.Id == movieId);
            if (movie == null)
            {
                movie = NowPlaying.Find(x => x.Id == movieId);
                if (movie == null)
                {
                    movie = Popular.Find(x => x.Id == movieId);
                    if (movie == null)
                        movie = TopRated.Find(x => x.Id == movieId);
                }
            }
            if (movie != null)
            {
                summaryMap.Add("Overview", movie.Overview);
                if (movie.ReleaseDate.HasValue)
                    summaryMap.Add("Year", movie.ReleaseDate.Value.Year.ToString());
                if (movie.GenreIds != null)
                    summaryMap.Add("Genres", string.Join(", ", movie.GenreIds.Select(x => genreMap[x])));
                if (movie.ReleaseDate.HasValue)
                    summaryMap.Add("Release date", movie.ReleaseDate.Value.ToString("dd MMMM yyyy"));
                summaryMap.Add("Language", languageMap[movie.OriginalLanguage]);
            }
            else
            {
                var movie1 = tmdbClient.GetMovieAsync(movieId).Result;
                summaryMap.Add("Overview", movie1.Overview);
                if (movie1.ReleaseDate.HasValue)
                    summaryMap.Add("Year", movie1.ReleaseDate.Value.Year.ToString());
                if (movie1.Genres != null)
                    summaryMap.Add("Genres", string.Join(", ", movie1.Genres.Select(x => genreMap[x.Id])));
                if (movie1.ReleaseDate.HasValue)
                    summaryMap.Add("Release date", movie1.ReleaseDate.Value.ToString("dd MMMM yyyy"));
                summaryMap.Add("Language", languageMap[movie1.OriginalLanguage]);
            }
            return summaryMap;
        }

        public string GetGenreText(int genreId)
        {
            return genreMap[genreId];
        }
    }
}
