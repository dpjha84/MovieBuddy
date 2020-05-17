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
        public static void Init(ILocalDataProvider provider, bool fake = false)
        {
            if (Instance == null)
            {
                Instance = new MovieManager(provider);                
                Instance.Load();
            }
        }

        TMDbClient tmdbClient;
        public static MovieManager Instance { get; private set; }

        public ILocalDataProvider LocalDataProvider;
        public Dictionary<string, TmdbMovie> MovieData = new Dictionary<string, TmdbMovie>();

        private MovieManager(ILocalDataProvider provider)
        {
            LocalDataProvider = provider;
            tmdbClient = new TMDbClient("c6b31d1cdad6a56a23f0c913e2482a31");
        }
        Dictionary<int, string> GenresMap;
        public void Load()
        {
            GenresMap = new Dictionary<int, string>();
            var list = tmdbClient.GetMovieGenresAsync().Result;
            foreach (var g in list)
            {
                GenresMap.Add(g.Id, g.Name);
            }
            Upcoming = tmdbClient.GetMovieUpcomingListAsync().Result.Results;
            NowPlaying = tmdbClient.GetMovieNowPlayingListAsync().Result.Results;
            Trending = tmdbClient.GetTrendingMoviesAsync(TimeWindow.Week).Result.Results;
            TopRated = tmdbClient.GetMovieTopRatedListAsync().Result.Results;
            Popular = tmdbClient.GetMoviePopularListAsync().Result.Results;
            //..Select(x => new TmdbMovie
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
            //    BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
            //    Overview = x.Overview
            //}).ToList();
            //NowPlaying = tmdbClient.GetMovieNowPlayingListAsync().Result.Results.Select(x => new TmdbMovie
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
            //    BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
            //    Overview = x.Overview
            //}).ToList();
            //Trending = tmdbClient.GetTrendingMoviesAsync(TimeWindow.Week).Result.Results.Select(x => new TmdbMovie
            //{
            //    Id = x.Id,
            //    Title = x.Title,
            //    PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
            //    BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
            //    Overview = x.Overview
            //}).ToList();
        }

        public List<TMDbLib.Objects.Search.SearchMovie> Upcoming { get; private set; }
        public List<TMDbLib.Objects.Search.SearchMovie> NowPlaying { get; private set; }
        public List<TMDbLib.Objects.Search.SearchMovie> Trending { get; private set; }

        public List<TMDbLib.Objects.Search.SearchMovie> Popular { get; private set; }

        public List<TMDbLib.Objects.Search.SearchMovie> TopRated { get; private set; }

        public string GetTrailer(int movieId, string name)
        {
            var videos = tmdbClient.GetMovieVideosAsync(movieId).Result.Results;
            if (videos == null || videos.Count == 0) return null;
            return videos.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Key)).Key;
        }

        public List<TmdbMovie> GetReleased()
        {
            var newList = MovieData.Values.Where(c => c.ReleaseDate <= DateTime.Now).ToList();
            return newList.OrderByDescending(m => m.ReleaseDate).ToList();
        }

        public List<TMDbLib.Objects.Search.SearchMovie> GetAllTrailers()
        {
            //var list = MovieData.Values.Where(c => !string.IsNullOrWhiteSpace(c.Trailer) && c.ReleaseDate > DateTime.Now).ToList();
            //return list.OrderBy(m => m.ReleaseDate).ToList();
            return new List<TMDbLib.Objects.Search.SearchMovie>();
        }

        public bool IsMoviePresent(string movieName)
        {
            var movie = MovieData.Values.Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            return movie != null;
        }

        public Credits GetCast(int movieId, string name)
        {
            return new Credits { Cast = new List<Cast>() };
        }

        public TMDbLib.Objects.Movies.Credits GetCastAndCrew(int movieId) => tmdbClient.GetMovieCreditsAsync(movieId).Result;

        public List<TMDbLib.Objects.Search.SearchMovie> GetSimilar(int movieId) => tmdbClient.GetMovieSimilarAsync(movieId).Result.Results;

        public List<TMDbLib.Objects.Reviews.ReviewBase> GetReviews(int movieId) => tmdbClient.GetMovieReviewsAsync(movieId).Result.Results;

        public MovieCredits GetMovieCredits(int personId) => tmdbClient.GetPersonMovieCreditsAsync(personId).Result;

        public string GetGenres(TmdbMovie movie, int count = 0)
        {
            var genres = movie.GenreText;
            if (count == 0 || string.IsNullOrWhiteSpace(genres))
                return genres;
            var splits = genres.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Take(count);
            var join = string.Join(", ", splits);
            return join;
        }

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
                        if(movie == null)
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
                    result.Add(string.Join(", ", movie.GenreIds.Select(x => GenresMap[x])));
                }
                result.Add("\nLanguage:");
                result.Add(movie.OriginalLanguage);
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
                    result.Add(string.Join(", ", movie1.Genres.Select(x => GenresMap[x.Id])));
                }
                result.Add("\nLanguage:");
                result.Add(movie1.OriginalLanguage);
            }
            return result;
        }
    }
}
