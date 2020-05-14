using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMDbLib.Objects.Search;
using RecyclerViewer.Data;

namespace RecyclerViewer
{
    public class ToiRating : IRating
    {
        Dictionary<string, string> ToiRatingCache = new Dictionary<string, string>();
        public Dictionary<string, string> Cache
        {
            get => ToiRatingCache;
        }

        public string GetRating(string movieName)
        {
            throw new NotImplementedException();
        }
    }
    public interface IRating
    {
        string GetRating(string movieName);
        Dictionary<string, string> Cache { get; }
    }

    public class Cache
    {
        static Dictionary<int, string> SummaryCache = new Dictionary<int, string>();
        static Dictionary<int, TmdbMovie> MovieCache = new Dictionary<int, TmdbMovie>();
        static Dictionary<int, string> TrailerCache = new Dictionary<int, string>();
        static Dictionary<int, CastDetail> CastCache = new Dictionary<int, CastDetail>();
        static Dictionary<string, string> ToiRatingCache = new Dictionary<string, string>();
        static Dictionary<string, string> HtRatingCache = new Dictionary<string, string>();
        static Dictionary<string, string> ImdbRatingCache = new Dictionary<string, string>();

        static void AddToCache<TKey, TValue>(Dictionary<TKey, TValue> cache, TKey key, TValue value, string suffix, bool skipLocalSave = false)
        {
            if (cache.ContainsKey(key))
                cache[key] = value;
            else
                cache.Add(key, value);
            if (!skipLocalSave)
            {
                var data = JsonConvert.SerializeObject(value);
                LocalDataProvider.Set($"{key}{suffix}", data);
            }
        }

        static TValue GetFromCache<TKey, TValue>(Dictionary<TKey, TValue> cache, TKey key, string suffix)
        {
            if (cache.ContainsKey(key))
                return cache[key];
            else
            {
                var data = LocalDataProvider.Get($"{key}{suffix}");
                if (!string.IsNullOrWhiteSpace(data))
                {
                    TValue obj;
                    if (typeof(TValue) == typeof(string))
                        obj = (TValue)Convert.ChangeType(data, typeof(TValue));
                    else
                        obj = JsonConvert.DeserializeObject<TValue>(data);                    
                    cache.Add(key, obj);
                    return obj;
                }
            }
            return default(TValue);
        }

        public static void AddToToiRating(string key, string value, bool skipLocalSave = false)
        {
            AddToCache(ToiRatingCache, key, value, "tr", skipLocalSave);
        }

        public static string GetToiRating(string key)
        {
            return GetFromCache(ToiRatingCache, key, "tr");
        }

        public static void AddToHtRating(string key, string value, bool skipLocalSave = false)
        {
            AddToCache(HtRatingCache, key, value, "hr", skipLocalSave);
        }

        public static string GetHtRating(string key)
        {
            return GetFromCache(HtRatingCache, key, "hr");
        }

        public static void AddToImdbRating(string key, string value, bool skipLocalSave = false)
        {
            AddToCache(ImdbRatingCache, key, value, "ir", skipLocalSave);
        }

        public static string GetImdbRating(string key)
        {
            return GetFromCache(ImdbRatingCache, key, "ir");
        }

        public static void AddToMovie(int key, TmdbMovie value, bool skipLocalSave = false)
        {
            AddToCache(MovieCache, key, value, "m", skipLocalSave);
        }

        public static TmdbMovie GetMovie(int key)
        {
            return GetFromCache(MovieCache, key, "m");
        }

        public static void AddToTrailer(int key, string value, bool skipLocalSave = false)
        {
            AddToCache(TrailerCache, key, value, "t", skipLocalSave);
        }

        public static string GetTrailer(int key)
        {
            return GetFromCache(TrailerCache, key, "t");
        }

        public static void AddToCast(int key, CastDetail value, bool skipLocalSave = false)
        {
            AddToCache(CastCache, key, value, "c", skipLocalSave);
        }

        public static CastDetail GetCast(int key)
        {
            return GetFromCache(CastCache, key, "c");
        }
    }
}