﻿using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using IMDbApiLib.Models;
using MovieBuddy.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.People;
using TCast = TMDbLib.Objects.Movies.Cast;
using TFMovie = TMDbLib.Objects.Movies.Movie;
using TReview = TMDbLib.Objects.Reviews.ReviewBase;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class MovieManager
    {
        public static readonly Dictionary<int, string> GenreMap = new Dictionary<int, string>();
        public static readonly Dictionary<string, int> GenreTextToIdMap = new Dictionary<string, int>();
        private readonly Dictionary<string, string> languageMap = new Dictionary<string, string>();
        private readonly TClient tClient;
        private readonly ImdbClient imdbClient;
        private readonly OClient oClient;

        private readonly YouTubeService youTubeClient;
        private readonly string tmdbApiKey;
        private int personMoviesTotal = 0;
        private MovieCredits movieCredits;
        private int totalCast = 0;
        private TMDbLib.Objects.Movies.Credits Credits;
        private int popularTotalPage = int.MaxValue, topRatedTotalPage = int.MaxValue;

        public static MovieManager Instance { get; private set; }

        private static bool loaded = false;

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
            tClient = new TClient();
            imdbClient = new ImdbClient();
            oClient = new OClient();
            tmdbApiKey = tClient.Key;
            youTubeClient = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDFaNumZ4ZE3hDY-yDQi9VY_WDEYrS3xvo",
                ApplicationName = "Google Play Android Developer"
            });
        }

        private T GetCachedOrWebData<T>(string key, Func<T> getDataFromWeb = null)
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

        private void Load()
        {
            foreach (var g in GetCachedOrWebData(LocalCache.GenresKey, () => tClient.GetMovieGenresAsync().Result))
            {
                GenreMap.Add(g.Id, g.Name);
                GenreTextToIdMap.Add(g.Name, g.Id);
            }

            foreach (var l in GetCachedOrWebData(LocalCache.LanguagesKey, () => tClient.GetLanguagesAsync().Result))
                languageMap.Add(l.Iso_639_1, l.EnglishName);

            _ = MoviesWithTrailer.Instance;
            Task.Run(() =>
            {
                try
                {

                    //GetVideos(0, null, null, null, 2);
                    //GetVideos(0, null, null, null, 3);
                    //var nowPlaying = GetNowPlaying(1);
                    //var upcoming = GetUpcoming(1);
                    //GetVideos(0, null, null, null, 1);                    
                    GetPopular(1);
                    GetTopRated(1);
                    var imdbList = GetImdbTop250(1);
                    for (int i = 1; i < 17; i++)
                    {
                        GetImdbTop250(i + 1);
                    }
                    //var list = nowPlaying.Concat(upcoming).Concat(popularRes).Concat(topRatedRes).Concat(imdbList);
                    //foreach (var item in list)
                    //{
                    //    GetFullOverview(item.Id);
                    //    GetCastAndCrew(item.Id);
                    //    //GetVideos(item.Id, item.OriginalTitle, item.ReleaseDate, item.OriginalLanguage);
                    //    GetReviews(item.Id);
                    //    GetSimilar(item.Id, 1);
                    //}
                }
                catch (Exception ex)
                {
                    TClient.LogError(ex);
                }
            });
        }

        private List<Top250DataDetail> top250Movies = null;
        public List<TSMovie> GetImdbTop250(int page)
        {
            if (top250Movies == null)
                top250Movies = imdbClient.GetTop250();
            return top250Movies.Skip((page - 1) * 15).Take(15).Select(m =>
            {
                var ms = CacheRepo.TmdbStaticByImdbId.GetOrCreate(m.Id, () =>
                {
                    var movie = CacheRepo.MovieByImdbId.GetOrCreate(m.Id, () => tClient.GetByImdbId(m.Id));
                    return new TmdbStatic { Id = movie.Id, Backdrop = movie.BackdropPath, Poster = movie.PosterPath };
                });
                return new TSMovie
                {
                    Id = ms.Id,
                    Title = m.Title,
                    PosterPath = ms.Poster,
                    BackdropPath = ms.Backdrop,
                    OriginalTitle = m.IMDbRating.ToString()
                };
            }).ToList();
        }

        public List<TSMovie> SearchMovie(string query, int page) => string.IsNullOrWhiteSpace(query) ? null : tClient.SearchMovieAsync(query, page).Result.Results;

        public List<TMDbLib.Objects.Search.SearchPerson> SearchPerson(string query, int page) => string.IsNullOrWhiteSpace(query) ? null : tClient.SearchPersonAsync(query, page).Result.Results;

        public List<PersonResult> GetPopularPersons(int page) => tClient.GetPersonListAsync(PersonListType.Popular, page).Result.Results;

        public List<TSMovie> ExploreMovies(IList<int> exploreInfo, int page)
        {
            var startYear = exploreInfo[0];
            var endYear = exploreInfo[1];
            if (startYear > endYear) return null;
            var genreId = exploreInfo[2];
            return tClient.Discover(startYear, endYear, genreId, page);
        }

        public List<TSMovie> GetUpcoming(int page) => UpcomingMovies.Instance.Get(page);

        public List<TSMovie> GetNowPlaying(int page) => NowPlayingMovies.Instance.Get(page);

        private List<TSMovie> popularRes;
        private List<TSMovie> topRatedRes;

        public List<TSMovie> GetPopular(int page)
        {
            if (page > popularTotalPage) return null;
            if (page == 1)
            {
                if (popularRes == null)
                {
                    var res = tClient.GetMoviePopularListAsync(null, page).Result;
                    popularTotalPage = res.TotalPages;
                    popularRes = res.Results;
                }
                return popularRes;
            }
            var res1 = tClient.GetMoviePopularListAsync(null, page).Result;
            popularTotalPage = res1.TotalPages;
            return res1.Results;
        }

        public List<TSMovie> GetTopRated(int page)
        {
            if (page > topRatedTotalPage) return null;
            if (page == 1)
            {
                if (topRatedRes == null)
                {
                    var res = tClient.GetMovieTopRatedListAsync(null, page).Result;
                    topRatedTotalPage = res.TotalPages;
                    topRatedRes = res.Results;
                }
                return topRatedRes;
            }
            var res1 = tClient.GetMovieTopRatedListAsync(null, page).Result;
            topRatedTotalPage = res1.TotalPages;
            return res1.Results;
        }

        private List<VideoData> GetTrailer(int movieId, int page)
        {
            if (movieId == 0)
            {
                return MoviesWithTrailer.Instance.Get(page);
                //var videos = LoadTrailers(GetNowPlaying(page));
                //if(videos.Count < 5)
                //    videos.AddRange(LoadTrailers(GetUpcoming(page)));
                //return videos;
            }
            else
            {
                var videos = tClient.GetMovieVideosAsync(movieId).Result.Results;
                if (videos == null || videos.Count == 0) return null;
                return videos.Where(x => !string.IsNullOrWhiteSpace(x.Key)).Select(x => new VideoData { VideoId = x.Key }).ToList();
            }
        }

        public List<VideoData> GetVideos(int movieId, string movieName, DateTime? releaseDate, string lang, int page = 1)
        {
            try
            {
                var key = movieId == 0 ? $"trailers_{Globals.Language}_{page}" : $"trailers_{movieId}";
                //return CacheRepo.Videos.GetOrCreate(key, () =>
                //{
                var videos = GetTrailer(movieId, page);
                return videos ?? null;
                //});
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Person GetPerson(int castId)
        {
            return tClient.GetPersonAsync(castId).Result;
        }

        public List<TCast> GetCastAndCrew(int movieId, int page = 1)
        {
            if (page == 1)
            {
                Credits = CacheRepo.Casts.GetOrCreate(movieId.ToString(), () => tClient.GetMovieCreditsAsync(movieId).Result);
                totalCast = Credits.Cast.Count;
            }
            if (page > totalCast / 15 + 1) return null;
            return Credits.Cast.Skip((page - 1) * 15).Take(15).ToList();
        }

        public List<TSMovie> GetSimilar(int movieId, int page) => CacheRepo.Similar.GetOrCreate($"{movieId}-{page}", () => tClient.GetMovieSimilarAsync(movieId, page).Result.Results);

        private readonly CacheDictionary<int, TFMovie> movieCache = new CacheDictionary<int, TFMovie>(100, new LruRemovalStrategy<int>());
        public List<TSMovie> GetMovies(List<int> movieIds)
        {
            //TODO - Optimize
            var result = new List<TFMovie>();
            foreach (var movieId in movieIds)
            {
                if (movieCache.ContainsKey(movieId))
                    result.Add(movieCache[movieId]);
                else
                {
                    var movie = tClient.GetMovieAsync(movieId).Result;
                    movieCache.Add(movieId, movie);
                    result.Add(movie);
                }
            }
            return result.Select(x => new TSMovie
            {
                Id = x.Id,
                Title = x.Title,
                PosterPath = x.PosterPath,
                GenreIds = x.Genres.Select(z => z.Id).ToList()
            }).ToList();
        }

        public List<TReview> GetReviews(int movieId) => CacheRepo.Reviews.GetOrCreate(movieId.ToString(), () => tClient.GetMovieReviewsAsync(movieId).Result.Results);

        public List<MovieRole> GetMovieCredits(int personId, int page)
        {
            if (page == 1)
            {
                movieCredits = CacheRepo.Credits.GetOrCreate(personId.ToString(), () => tClient.GetPersonMovieCreditsAsync(personId).Result);
                movieCredits.Cast = movieCredits.Cast.Where(z => z.ReleaseDate.HasValue).OrderByDescending(x => x.ReleaseDate).ToList();
                personMoviesTotal = movieCredits.Cast.Count;
            }
            if (page > personMoviesTotal / 15 + 1) return null;
            return movieCredits.Cast.Skip((page - 1) * 15).Take(15).ToList();
        }

        public Dictionary<string, string> GetFullOverview(int movieId)
        {
            return CacheRepo.Summary.GetOrCreate(movieId.ToString(), () =>
            {
                Dictionary<string, string> summaryMap = new Dictionary<string, string>();
                var movie = tClient.GetMovieAsync(movieId).Result;
                if (!string.IsNullOrWhiteSpace(movie.Tagline))
                    summaryMap.Add("Tagline", movie.Tagline);
                if (!string.IsNullOrWhiteSpace(movie.Overview))
                    summaryMap.Add("Overview", movie.Overview);
                if (movie.ReleaseDate.HasValue)
                    summaryMap.Add("Year", movie.ReleaseDate.Value.Year.ToString());
                if (movie.Genres != null)
                    summaryMap.Add("Genres", string.Join(", ", movie.Genres.Select(x => GenreMap[x.Id])));
                if (movie.ReleaseDate.HasValue)
                    summaryMap.Add("Release date", movie.ReleaseDate.Value.ToString("dd MMMM yyyy"));
                if (movie.Runtime.HasValue)
                    summaryMap.Add("Runtime", $"{movie.Runtime.Value}m");
                if (movie.Budget != 0)
                    summaryMap.Add("Budget", $"${movie.Budget}");
                if (movie.VoteAverage != 0)
                    summaryMap.Add("TmdbRating", $"{movie.VoteAverage * 10}%");
                if (!string.IsNullOrWhiteSpace(movie.ImdbId) && movie.ImdbId != "0")
                {
                    var item = oClient.GetItem(movie.ImdbId);
                    if (item != null)
                    {
                        //summaryMap.Add("ImdbRating", item.ImdbRating);
                        if (item.Ratings.Count > 0)
                        {
                            summaryMap.Add("ImdbRating", item.Ratings[0].Value.Replace("/10", ""));
                        }
                        if (item.Ratings.Count > 1)
                        {
                            //var val = item.Ratings[1].Value.Replace("/100", "").Replace("%", "");
                            summaryMap.Add("RottenTomatoesRating", $"{item.Ratings[1].Value.Replace("/100", "").Replace("%", "")}%");
                        }
                    }
                }
                summaryMap.Add("Language", languageMap[movie.OriginalLanguage]);
                return summaryMap;
            });
        }

        public string GetGenreText(int genreId) => GenreMap.ContainsKey(genreId) ? GenreMap[genreId] : "";
    }
}
