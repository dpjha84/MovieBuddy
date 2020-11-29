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
using TMDbLib.Objects.Reviews;
using TCredits = TMDbLib.Objects.Movies.Credits;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    //public class CacheRepo
    //{
    //    Dictionary<string, Cache<T>> caches = new Dictionary<string, Cache<T>>();
    //}
    public class Cache<T>
    {
        private readonly MemoryCache _cache;
        private readonly string _cacheName;
        public Cache(string cacheName)
        {
            _cacheName = cacheName;
            var cacheInDisk = LocalCache.Instance.Get(_cacheName);
            if (!string.IsNullOrWhiteSpace(cacheInDisk))
            {
                try
                {
                    //_cache = JsonConvert.DeserializeObject<MemoryCache>(cacheInDisk);
                    using (FileStream fileStream = new FileStream("MovieSummary123", FileMode.Open))
                    {
                        IFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        _cache = (MemoryCache)bf.Deserialize(fileStream);
                    }
                }
                catch (Exception)
                {
                    _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 40 });
                }                
            }
            else
                _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 40 });            
        }

        public T GetOrCreate(object key, Func<T> createItem)
        {
            if (!_cache.TryGetValue(key, out T cacheEntry))
            {
                cacheEntry = createItem();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(key, cacheEntry, cacheEntryOptions);
                //LocalCache.Instance.Set(_cacheName, JsonConvert.SerializeObject(_cache.ToDictionary()));

                using (FileStream fileStream = new FileStream("MovieSummary123", FileMode.Create))
                {
                    IFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bf.Serialize(fileStream, _cache);
                }
            }
            return cacheEntry;
        }
    }

    public class Cache2<T>
    {
        private readonly ICacheManager<T> _cache;
        private readonly string _cacheName;
        public Cache2(string cacheName)
        {
            _cacheName = cacheName;
            var cacheInDisk = LocalCache.Instance.Get(_cacheName);
            if (!string.IsNullOrWhiteSpace(cacheInDisk))
            {
                try
                {
                    _cache = JsonConvert.DeserializeObject<BaseCacheManager<T>>(cacheInDisk);
                }
                catch (Exception ex)
                {
                    _cache = CacheFactory.Build<T>(_cacheName, settings =>
                    {
                        settings.WithDictionaryHandle(true)                      
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(5))
                       ;
                    });
                }
            }
            else
                _cache = CacheFactory.Build<T>(_cacheName, settings =>
                {
                    settings.WithDictionaryHandle(true)
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(5));
                });
        }

        public T GetOrCreate(object key, Func<T> createItem)
        {
            var cacheEntry = _cache.Get<T>(key.ToString());
            if (cacheEntry == null)
            {
                cacheEntry = createItem();
                _cache.Add(key.ToString(), cacheEntry);
                LocalCache.Instance.Set(_cacheName, JsonConvert.SerializeObject(_cache));
            }
            return cacheEntry;
        }
    }

    public class CacheRepo
    {
        static Lazy<Cache3<Dictionary<string, string>>> summary = new Lazy<Cache3<Dictionary<string, string>>>(() => new Cache3<Dictionary<string, string>>("MovieSummaryCache"));
        static Lazy<Cache3<List<string>>> videos = new Lazy<Cache3<List<string>>>(() => new Cache3<List<string>>("MovieVideosCache"));
        static Lazy<Cache3<TCredits>> casts = new Lazy<Cache3<TCredits>>(() => new Cache3<TCredits>("MovieCastsCache"));
        static Lazy<Cache3<List<ReviewBase>>> reviews = new Lazy<Cache3<List<ReviewBase>>>(() => new Cache3<List<ReviewBase>>("MovieReviewsCache"));
        static Lazy<Cache3<List<TSMovie>>> similar = new Lazy<Cache3<List<TSMovie>>>(() => new Cache3<List<TSMovie>>("MovieSimilarCache"));

        public static Cache3<Dictionary<string, string>> Summary => summary.Value;
        public static Cache3<List<string>> Videos => videos.Value;
        public static Cache3<TCredits> Casts => casts.Value;
        public static Cache3<List<ReviewBase>> Reviews => reviews.Value;
        public static Cache3<List<TSMovie>> Similar => similar.Value;
    }

    public class Cache3<T>
    {
        private static Dictionary<string, CacheItem<T>> _cache;
        private readonly string _cacheName;
        public Cache3(string cacheName)
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
            if (!_cache.TryGetValue(key, out CacheItem<T> cacheEntry) || DateTime.Now.Subtract(cacheEntry.TimeAdded) > TimeSpan.FromMinutes(5))
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