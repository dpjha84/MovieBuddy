using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Reviews;
using TCredits = TMDbLib.Objects.Movies.Credits;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    public class CacheRepo
    {
        private static readonly Lazy<Cache<Dictionary<string, string>>> summary = new Lazy<Cache<Dictionary<string, string>>>(() => new Cache<Dictionary<string, string>>("MovieSummaryCache"));
        private static readonly Lazy<Cache<List<string>>> videos = new Lazy<Cache<List<string>>>(() => new Cache<List<string>>("MovieVideosCache"));
        private static readonly Lazy<Cache<TCredits>> casts = new Lazy<Cache<TCredits>>(() => new Cache<TCredits>("MovieCastsCache"));
        private static readonly Lazy<Cache<List<ReviewBase>>> reviews = new Lazy<Cache<List<ReviewBase>>>(() => new Cache<List<ReviewBase>>("MovieReviewsCache"));
        private static readonly Lazy<Cache<List<TSMovie>>> similar = new Lazy<Cache<List<TSMovie>>>(() => new Cache<List<TSMovie>>("MovieSimilarCache"));
        private static readonly Lazy<Cache<TSMovie>> movieByImdbId = new Lazy<Cache<TSMovie>>(() => new Cache<TSMovie>("MovieByImdbId"));
        private static readonly Lazy<Cache<TmdbStatic>> tmdbStaticByImdbId = new Lazy<Cache<TmdbStatic>>(() => new Cache<TmdbStatic>("TmdbStaticByImdbId", true));
        private static readonly Lazy<Cache<MovieCredits>> credits = new Lazy<Cache<MovieCredits>>(() => new Cache<MovieCredits>("PersonMovies"));
        private static readonly Lazy<Cache<List<TSMovie>>> nowPlaying = new Lazy<Cache<List<TSMovie>>>(() => new Cache<List<TSMovie>>("NowPlayingMovies"));
        private static readonly Lazy<Cache<List<TSMovie>>> upcoming = new Lazy<Cache<List<TSMovie>>>(() => new Cache<List<TSMovie>>("UpcomingMovies"));

        public static Cache<Dictionary<string, string>> Summary => summary.Value;
        public static Cache<List<string>> Videos => videos.Value;
        public static Cache<TCredits> Casts => casts.Value;
        public static Cache<List<ReviewBase>> Reviews => reviews.Value;
        public static Cache<List<TSMovie>> Similar => similar.Value;

        public static Cache<List<TSMovie>> NowPlaying => nowPlaying.Value;
        public static Cache<List<TSMovie>> Upcoming => upcoming.Value;

        public static Cache<MovieCredits> Credits => credits.Value;

        public static Cache<TSMovie> MovieByImdbId => movieByImdbId.Value;
        public static Cache<TmdbStatic> TmdbStaticByImdbId => tmdbStaticByImdbId.Value;
    }

    public class TmdbStatic
    {
        public int Id { get; set; }

        public string Poster { get; set; }

        public string Backdrop { get; set; }
    }
    public class Cache<T>
    {
        private static ConcurrentDictionary<string, CacheItem<T>> _cache;
        private readonly string _cacheName;
        private readonly bool _noExpiry;
        public Cache(string cacheName, bool noExpiry = false)
        {
            _cacheName = cacheName;
            _noExpiry = noExpiry;
            var cacheInDisk = LocalCache.Instance.Get(_cacheName);
            if (!string.IsNullOrWhiteSpace(cacheInDisk))
            {
                try
                {
                    _cache = JsonConvert.DeserializeObject<ConcurrentDictionary<string, CacheItem<T>>>(cacheInDisk);
                }
                catch (Exception)
                {
                    _cache = new ConcurrentDictionary<string, CacheItem<T>>();
                }
            }
            else
                _cache = new ConcurrentDictionary<string, CacheItem<T>>();
        }

        public T GetOrCreate(string key, Func<T> createItem)
        {
            if (!_cache.TryGetValue(key, out CacheItem<T> cacheEntry) || (!_noExpiry && DateTime.Now.Subtract(cacheEntry.TimeAdded) > TimeSpan.FromHours(24)))
            {
                cacheEntry = new CacheItem<T>
                {
                    Data = createItem(),
                    TimeAdded = DateTime.Now
                };
                //_cache.Remove(key);
                _cache.AddOrUpdate(key, cacheEntry, (k, v) => cacheEntry);
                LocalCache.Instance.Set(_cacheName, JsonConvert.SerializeObject(_cache));
            }
            return cacheEntry.Data;
        }
    }

    public class CacheItem<T>
    {
        public DateTime TimeAdded;
        public T Data;
    }
}