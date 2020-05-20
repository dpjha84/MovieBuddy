using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Trending;

namespace MovieBuddy
{
    public class MovieManager
    {
        Dictionary<int, string> genreMap;
        Dictionary<string, string> languageMap;
        readonly TMDbClient tmdbClient;
        private List<TMDbLib.Objects.Search.SearchMovie> upcoming;
        private List<TMDbLib.Objects.Search.SearchMovie> nowPlaying;
        private List<TMDbLib.Objects.Search.SearchMovie> trending;
        private List<TMDbLib.Objects.Search.SearchMovie> popular;
        private List<TMDbLib.Objects.Search.SearchMovie> topRated;
        public static MovieManager Instance { get; private set; }
        public Dictionary<string, TmdbMovie> MovieData = new Dictionary<string, TmdbMovie>();

        public static void Init(ILocalDataProvider provider, bool fake = false)
        {
            if (Instance == null)
            {
                Instance = new MovieManager(provider);
                Instance.Load();
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
        }

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
        public List<TMDbLib.Objects.Search.SearchMovie> Trending
        {
            get
            {
                if (trending == null)
                    trending = tmdbClient.GetTrendingMoviesAsync(TimeWindow.Day).Result.Results;
                return trending;
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

        public TMDbLib.Objects.Movies.Credits GetCastAndCrew(int movieId) => tmdbClient.GetMovieCreditsAsync(movieId).Result;

        public List<TMDbLib.Objects.Search.SearchMovie> GetSimilar(int movieId) => tmdbClient.GetMovieSimilarAsync(movieId).Result.Results;

        public List<TMDbLib.Objects.Reviews.ReviewBase> GetReviews(int movieId) => tmdbClient.GetMovieReviewsAsync(movieId).Result.Results;

        public MovieCredits GetMovieCredits(int personId) => tmdbClient.GetPersonMovieCreditsAsync(personId).Result;

        public List<string> GetFullOverview(int movieId)
        {
            var result = new List<string>();
            var movie = Upcoming.Find(x => x.Id == movieId);
            if (movie == null)
            {
                movie = NowPlaying.Find(x => x.Id == movieId);
                if (movie == null)
                {
                    movie = Trending.Find(x => x.Id == movieId);
                    if (movie == null)
                    {
                        movie = Popular.Find(x => x.Id == movieId);
                        if (movie == null)
                            movie = TopRated.Find(x => x.Id == movieId);
                    }
                }
            }
            if (movie != null)
            {
                result.Add("Overview:");
                result.Add(movie.Overview);
                if (movie.ReleaseDate.HasValue)
                {
                    result.Add("\nYear:");
                    result.Add(movie.ReleaseDate.Value.Year.ToString());
                }
                if (movie.GenreIds != null)
                {
                    result.Add("\nGenres:");
                    result.Add(string.Join(", ", movie.GenreIds.Select(x => genreMap[x])));
                }
                result.Add("\nLanguage:");
                result.Add(languageMap[movie.OriginalLanguage]);
            }
            else
            {
                var movie1 = tmdbClient.GetMovieAsync(movieId).Result;
                result.Add("Overview:");
                result.Add(movie1.Overview);
                if (movie1.ReleaseDate.HasValue)
                {
                    result.Add("\nYear:");
                    result.Add(movie1.ReleaseDate.Value.Year.ToString());
                }
                if (movie1.Genres != null)
                {
                    result.Add("\nGenres:");
                    result.Add(string.Join(", ", movie1.Genres.Select(x => genreMap[x.Id])));
                }
                result.Add("\nLanguage:");
                result.Add(languageMap[movie1.OriginalLanguage]);
            }
            return result;

        }

        public string GetGenreText(int genreId)
        {
            return genreMap[genreId];
        }
    }
}
