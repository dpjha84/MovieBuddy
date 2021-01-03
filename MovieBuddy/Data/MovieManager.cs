using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MovieBuddy.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.People;
using TCast = TMDbLib.Objects.Movies.Cast;
using TReview = TMDbLib.Objects.Reviews.ReviewBase;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;
using TFMovie = TMDbLib.Objects.Movies.Movie;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using IMDbApiLib.Models;

namespace MovieBuddy
{
    public class MovieManager
    {
        public static readonly Dictionary<int, string> GenreMap = new Dictionary<int, string>();
        public static readonly Dictionary<string, int> GenreTextToIdMap = new Dictionary<string, int>();
        readonly Dictionary<string, string> languageMap = new Dictionary<string, string>();
        readonly TClient tClient;
        readonly ImdbClient imdbClient;
        readonly OClient oClient;
        //readonly TMDbClient tmdbClient;
        readonly YouTubeService youTubeClient;
        string tmdbApiKey;
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
            foreach (var g in GetCachedOrWebData(LocalCache.GenresKey, () => tClient.GetMovieGenresAsync().Result))
            {
                GenreMap.Add(g.Id, g.Name);
                GenreTextToIdMap.Add(g.Name, g.Id);
            }

            foreach (var l in GetCachedOrWebData(LocalCache.LanguagesKey, () => tClient.GetLanguagesAsync().Result))
                languageMap.Add(l.Iso_639_1, l.EnglishName);

            var today = DateTime.Now;
            var endDate = today.ToString("yyyy-MM-dd");
            var startDate = today.Subtract(TimeSpan.FromDays(60)).ToString("yyyy-MM-dd");
            nowPlayingBaseUrl = BuildUrl(startDate, "release_date.desc", endDate);
            upcomingBaseUrl = BuildUrl(today.AddDays(1).ToString("yyyy-MM-dd"), "release_date.asc");
            filterNowPlaying = x => x.ReleaseDate >= DateTime.Parse(startDate) && x.ReleaseDate <= DateTime.Parse(endDate) && x.GenreIds?.Count > 0 && x.PosterPath != null;
            filterUpcoming = x => x.ReleaseDate > DateTime.Parse(endDate) && x.GenreIds?.Count > 0 && x.PosterPath != null;

            Task.Run(() =>
            {
                try
                {
                    GetNowPlaying(1);
                    GetUpcoming(1);
                    GetPopular(1);
                    GetTopRated(1);
                    var imdbList = GetImdbTop250(1);
                    for (int i = 1; i < 17; i++)
                    {
                        GetImdbTop250(i + 1);
                    }
                    var list = nowPlayingRes[Globals.Language].Concat(upcomingRes[Globals.Language]).Concat(popularRes).Concat(topRatedRes).Concat(imdbList);
                    foreach (var item in list)
                    {
                        GetFullOverview(item.Id);
                        GetCastAndCrew(item.Id);
                        GetVideos(item.Id, item.OriginalTitle, item.ReleaseDate, item.OriginalLanguage);
                        GetReviews(item.Id);
                        GetSimilar(item.Id, 1);
                    }                    
                }
                catch(Exception ex)
                {

                }                
            });
        }

        List<Top250DataDetail> top250Movies = null;
        public List<TSMovie> GetImdbTop250(int page)
        {
            if(top250Movies == null)
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

        public List<TSMovie> GetUpcoming(int page)
        {
            if (page == 1) upcomingTotalPage = int.MaxValue;
            if (page > upcomingTotalPage) return null;

            if (page == 1)
            {
                if (!upcomingRes.ContainsKey(Globals.Language))
                {
                    if (Globals.Language == "All") return tClient.GetMovieUpcomingListAsync(null, page).Result.Results;
                    var result = tClient.GetMoviesByUrl(upcomingBaseUrl, page);
                    upcomingTotalPage = result.TotalPages;
                    upcomingRes.AddOrUpdate(Globals.Language, result.Results.Where(filterUpcoming).ToList(), (k, v) => v);
                }
                return upcomingRes[Globals.Language];
            }
            if (Globals.Language == "All") return tClient.GetMovieUpcomingListAsync(null, page).Result.Results;
            var result1 = tClient.GetMoviesByUrl(upcomingBaseUrl, page);
            upcomingTotalPage = result1.TotalPages;
            return result1.Results.Where(filterUpcoming).ToList();
        }

        //public List<TSMovie> GetTrending(int page = 1) => page == 1 ? tmdbClient.GetTrendingMoviesAsync(TimeWindow.Week).Result.Results : null;

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

            if (page == 1)
            {
                if (!nowPlayingRes.ContainsKey(Globals.Language))
                {
                    if (Globals.Language == "All") return tClient.GetMovieNowPlayingListAsync(null, page).Result.Results;
                    var result = tClient.GetMoviesByUrl(nowPlayingBaseUrl, page);
                    nowPlayingTotalPage = result.TotalPages;
                    nowPlayingRes.AddOrUpdate(Globals.Language, result.Results.Where(filterNowPlaying).ToList(), (k, v) => v);
                }
                return nowPlayingRes[Globals.Language];
            }
            if (Globals.Language == "All") return tClient.GetMovieNowPlayingListAsync(null, page).Result.Results;
            var result1 = tClient.GetMoviesByUrl(nowPlayingBaseUrl, page);
            nowPlayingTotalPage = result1.TotalPages;
            return result1.Results.Where(filterNowPlaying).ToList();
        }
        List<TSMovie> popularRes;
        List<TSMovie> topRatedRes;
        readonly ConcurrentDictionary<string, List<TSMovie>> upcomingRes = new ConcurrentDictionary<string, List<TSMovie>>();
        readonly ConcurrentDictionary<string, List<TSMovie>> nowPlayingRes = new ConcurrentDictionary<string, List<TSMovie>>();

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

        public List<string> GetTrailer(int movieId)
        {
            var videos = tClient.GetMovieVideosAsync(movieId).Result.Results;
            if (videos == null || videos.Count == 0) return null;
            return videos.Where(x => !string.IsNullOrWhiteSpace(x.Key)).Select(x => x.Key).ToList();
        }

        public List<string> GetVideos(int movieId, string movieName, DateTime? releaseDate, string lang, int page = 1)
        {
            if (page > 1) return null;
            var videos = GetTrailer(movieId);
            return videos ?? null;
            //try
            //{
            //    return CacheRepo.Videos.GetOrCreate(movieId.ToString(), () =>
            //    {
            //        if (videoCache.ContainsKey(movieId)) return videoCache[movieId];
            //        var year = releaseDate.HasValue ? releaseDate.Value.Year : 0;
            //        var searchListRequest = youTubeClient.Search.List("snippet");
            //        searchListRequest.Q = year != 0 ? $"{movieName} {year} Full Movie" : $"{movieName} Full Movie";
            //        searchListRequest.Type = new List<string> { "video" };
            //        if (!string.IsNullOrWhiteSpace(lang))
            //            searchListRequest.RelevanceLanguage = lang;
            //        searchListRequest.MaxResults = releaseDate.HasValue && releaseDate.Value > DateTime.Now ? 1 : 5;
            //        var searchListResponse = searchListRequest.ExecuteAsync().Result;
            //        return searchListResponse.Items.Select(x => x.Id.VideoId).ToList();
            //    });
            //}
            //catch (Exception)
            //{
            //    var videos = GetTrailer(movieId);
            //    return videos ?? null;
            //}
        }

        Dictionary<int, List<string>> videoCache = new Dictionary<int, List<string>>();

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

        CacheDictionary<int, TFMovie> movieCache = new CacheDictionary<int, TFMovie>(100, new LruRemovalStrategy<int>());
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
                movieCredits.Cast = movieCredits.Cast.Where(z=> z.ReleaseDate.HasValue).OrderByDescending(x => x.ReleaseDate).ToList();
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
                if(movie.Budget != 0)
                    summaryMap.Add("Budget", $"${movie.Budget}");
                if (movie.VoteAverage != 0)
                    summaryMap.Add("TmdbRating", $"{movie.VoteAverage*10}%");
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
