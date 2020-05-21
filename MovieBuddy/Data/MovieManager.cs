using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;

namespace MovieBuddy
{
    public class MovieManager
    {
        Dictionary<int, string> genreMap;
        Dictionary<string, string> languageMap;
        readonly TMDbClient tmdbClient;
        private List<TMDbLib.Objects.Search.SearchMovie> upcoming;
        private List<TMDbLib.Objects.Search.SearchMovie> nowPlaying;
        private List<TMDbLib.Objects.Search.SearchMovie> popular;
        private List<TMDbLib.Objects.Search.SearchMovie> topRated;
        public static MovieManager Instance { get; private set; }

        public static void Init(ILocalDataProvider provider, bool fake = false)
        {
            if (Instance == null)
            {
                Instance = new MovieManager(provider);
                Instance.Load();// Async().GetAwaiter().GetResult();
            }
        }

        private MovieManager(ILocalDataProvider provider)
        {
            tmdbClient = new TMDbClient("c6b31d1cdad6a56a23f0c913e2482a31");
        }

        public void Load()
        {
            var list = tmdbClient.GetMovieGenresAsync().Result;
            var languages = tmdbClient.GetLanguagesAsync().Result;
            genreMap = new Dictionary<int, string>();
            languageMap = new Dictionary<string, string>();
            foreach (var g in list)
                genreMap.Add(g.Id, g.Name);
            foreach (var l in languages)
                languageMap.Add(l.Iso_639_1, l.EnglishName);
            _ = Upcoming;
            _ = NowPlaying;
        }

        //public async Task LoadAsync()
        //{
        //    var list = tmdbClient.GetMovieGenresAsync();
        //    var languages = tmdbClient.GetLanguagesAsync();
            

        //    genreMap = new Dictionary<int, string>();
        //    languageMap = new Dictionary<string, string>();

        //    await list;
        //    foreach (var g in list.Result)
        //        genreMap.Add(g.Id, g.Name);

        //    await languages;
        //    foreach (var l in languages.Result)
        //        languageMap.Add(l.Iso_639_1, l.EnglishName);

        //    var a1 = tmdbClient.GetMovieUpcomingListAsync();
        //    var a2 = tmdbClient.GetMovieNowPlayingListAsync();
        //    Task.WaitAll(a1, a2);
        //    upcoming = a1.Result.Results;
        //    nowPlaying = a2.Result.Results;
        //    //_ = Upcoming;
        //    //_ = NowPlaying;
        //    //_ = Trending;
        //    //_ = Popular;
        //    //_ = TopRated;
        //}

        public List<TMDbLib.Objects.Search.SearchMovie> Upcoming
        {
            get
            {
                if (upcoming == null)
                    upcoming = tmdbClient.GetMovieUpcomingListAsync().Result.Results;
                return upcoming;
            }
        }
        public List<TMDbLib.Objects.Search.SearchMovie> NowPlaying
        {
            get
            {
                if (nowPlaying == null)
                    nowPlaying = tmdbClient.GetMovieNowPlayingListAsync().Result.Results;
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

        public TMDbLib.Objects.Movies.Credits GetCastAndCrew(int movieId) => tmdbClient.GetMovieCreditsAsync(movieId).Result;

        public List<TMDbLib.Objects.Search.SearchMovie> GetSimilar(int movieId) => tmdbClient.GetMovieSimilarAsync(movieId).Result.Results.Take(10).ToList();

        public List<TMDbLib.Objects.Reviews.ReviewBase> GetReviews(int movieId) => tmdbClient.GetMovieReviewsAsync(movieId).Result.Results;

        public MovieCredits GetMovieCredits(int personId) => tmdbClient.GetPersonMovieCreditsAsync(personId).Result;

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
