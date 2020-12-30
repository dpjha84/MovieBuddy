using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CacheManager.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Reviews;
using TCredits = TMDbLib.Objects.Movies.Credits;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    public class CacheRepo
    {
        static Lazy<Cache<Dictionary<string, string>>> summary = new Lazy<Cache<Dictionary<string, string>>>(() => new Cache<Dictionary<string, string>>("MovieSummaryCache"));
        static Lazy<Cache<List<string>>> videos = new Lazy<Cache<List<string>>>(() => new Cache<List<string>>("MovieVideosCache"));
        static Lazy<Cache<TCredits>> casts = new Lazy<Cache<TCredits>>(() => new Cache<TCredits>("MovieCastsCache"));
        static Lazy<Cache<List<ReviewBase>>> reviews = new Lazy<Cache<List<ReviewBase>>>(() => new Cache<List<ReviewBase>>("MovieReviewsCache"));
        static Lazy<Cache<List<TSMovie>>> similar = new Lazy<Cache<List<TSMovie>>>(() => new Cache<List<TSMovie>>("MovieSimilarCache"));
        static Lazy<Cache<TSMovie>> movieByImdbId = new Lazy<Cache<TSMovie>>(() => new Cache<TSMovie>("MovieByImdbId"));
        static Lazy<Cache<MovieCredits>> credits = new Lazy<Cache<MovieCredits>>(() => new Cache<MovieCredits>("PersonMovies"));

        public static Cache<Dictionary<string, string>> Summary => summary.Value;
        public static Cache<List<string>> Videos => videos.Value;
        public static Cache<TCredits> Casts => casts.Value;
        public static Cache<List<ReviewBase>> Reviews => reviews.Value;
        public static Cache<List<TSMovie>> Similar => similar.Value;

        public static Cache<MovieCredits> Credits => credits.Value;

        public static Cache<TSMovie> MovieByImdbId => movieByImdbId.Value;
    }

    public class Cache<T>
    {
        private static Dictionary<string, CacheItem<T>> _cache;
        private readonly string _cacheName;
        public Cache(string cacheName)
        {
            _cacheName = cacheName;
            var cacheInDisk = LocalCache.Instance.Get(_cacheName);
            if (!string.IsNullOrWhiteSpace(cacheInDisk))
            {
                try
                {
                    _cache = JsonConvert.DeserializeObject<Dictionary<string, CacheItem<T>>>(cacheInDisk);
                }
                catch (Exception ex)
                {
                    _cache = new Dictionary<string, CacheItem<T>>();
                }
            }
            else
                _cache = new Dictionary<string, CacheItem<T>>();
        }

        public T GetOrCreate(string key, Func<T> createItem)
        {
            if (!_cache.TryGetValue(key, out CacheItem<T> cacheEntry) || DateTime.Now.Subtract(cacheEntry.TimeAdded) > TimeSpan.FromHours(12))
            {
                cacheEntry = new CacheItem<T>
                {
                    Data = createItem(),
                    TimeAdded = DateTime.Now
                };
                _cache.Remove(key);
                _cache.Add(key, cacheEntry);
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