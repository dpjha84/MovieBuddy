using Android.App;
using Android.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Essentials;

namespace MovieBuddy
{
    public static class Globals
    {
        public static Dictionary<string, string> LanguageMap = new Dictionary<string, string> {
            { "All", null },
            { "Arabic", "ar" },
            { "Bengali", "bn" },
            { "English", "en" },
            { "French", "fr" },
            { "German", "de" },
            { "Hindi", "hi" },
            { "Italian", "it" },
            { "Japanese", "ja" },
            { "Kannada", "kn" },
            { "Korean", "ko" },
            { "Malayalam", "ml" },
            { "Nepali", "ne" },
            { "Portuguese", "pt" },
            { "Russian", "ru" },
            { "Spanish", "es" },
            { "Tamil", "ta" },
            { "Telugu", "te" },
            { "Turkish", "tr" },
            { "Urdu", "ur" },
        };

        public static Dictionary<string, string> LanguageMap1 = new Dictionary<string, string> {
            { "ar", "Arabic" },
            { "bn", "Bengali" },
            { "en", "English" },
            { "fr", "French" },
            { "de", "German" },
            { "hi", "Hindi" },
            { "it", "Italian" },
            { "ja", "Japanese" },
            { "kn", "Kannada" },
            { "ko", "Korean" },
            { "ml", "Malayalam" },
            { "ne", "Nepali" },
            { "pt", "Portuguese" },
            { "ru", "Russian" },
            { "es", "Spanish" },
            { "ta", "Tamil" },
            { "te", "Telugu" },
            { "tr", "Turkish" },
            { "ur", "Urdu" }
        };

        static List<int> starredMovies = null;
        static List<int> watchedMovies = null;
        static List<int> toWatchMovies = null;

        public static void AddToStarredMovies(int movieId)
        {
            StarredMovies.Add(movieId);
            StarredMovies = starredMovies;
        }
        public static void RemoveFromStarredMovies(int movieId)
        {
            StarredMovies.Remove(movieId);
            StarredMovies = starredMovies;
        }
        public static void AddToWatchedMovies(int movieId)
        {
            WatchedMovies.Add(movieId);
            WatchedMovies = watchedMovies;
        }
        public static void RemoveFromWatchedMovies(int movieId)
        {
            WatchedMovies.Remove(movieId);
            WatchedMovies = watchedMovies;
        }
        public static void AddToWatchMovies(int movieId)
        {
            ToWatchMovies.Add(movieId);
            ToWatchMovies = toWatchMovies;
        }
        public static void RemoveFromToWatchMovies(int movieId)
        {
            ToWatchMovies.Remove(movieId);
            ToWatchMovies = toWatchMovies;
        }
        public static List<int> StarredMovies
        {
            get
            {
                if (starredMovies == null)
                {
                    var starredMoviesInDisk = LocalCache.Instance.Get("StarredMovies");
                    if (starredMoviesInDisk == null)
                    {
                        starredMoviesInDisk = JsonConvert.SerializeObject(new List<int>());
                        LocalCache.Instance.Set("StarredMovies", starredMoviesInDisk);
                    }
                    starredMovies = JsonConvert.DeserializeObject<List<int>>(starredMoviesInDisk);
                }
                return starredMovies;
            }
            set
            {
                starredMovies = value;
                LocalCache.Instance.Set("StarredMovies", JsonConvert.SerializeObject(starredMovies));
            }
        }
        public static List<int> WatchedMovies
        {
            get
            {
                if (watchedMovies == null)
                {
                    var watchedMoviesInDisk = LocalCache.Instance.Get("WatchedMovies");
                    if (watchedMoviesInDisk == null)
                    {
                        watchedMoviesInDisk = JsonConvert.SerializeObject(new List<int>());
                        LocalCache.Instance.Set("WatchedMovies", watchedMoviesInDisk);
                    }
                    watchedMovies = JsonConvert.DeserializeObject<List<int>>(watchedMoviesInDisk);
                }
                return watchedMovies;
            }
            set
            {
                watchedMovies = value;
                LocalCache.Instance.Set("WatchedMovies", JsonConvert.SerializeObject(watchedMovies));
            }
        }
        public static List<int> ToWatchMovies
        {
            get
            {
                if (toWatchMovies == null)
                {
                    var toWatchedMoviesInDisk = LocalCache.Instance.Get("ToWatchMovies");
                    if (toWatchedMoviesInDisk == null)
                    {
                        toWatchedMoviesInDisk = JsonConvert.SerializeObject(new List<int>());
                        LocalCache.Instance.Set("ToWatchMovies", toWatchedMoviesInDisk);
                    }
                    toWatchMovies = JsonConvert.DeserializeObject<List<int>>(toWatchedMoviesInDisk);
                }
                return toWatchMovies;
            }
            set
            {
                toWatchMovies = value;
                LocalCache.Instance.Set("ToWatchMovies", JsonConvert.SerializeObject(toWatchMovies));
            }
        }

        static string selectedLanguage = null;
        public static string Language
        {
            get
            {
                if (selectedLanguage == null)
                {
                    string languageInDisk;
                    try
                    {
                        languageInDisk = LocalCache.Instance.Get("MovieLanguage");
                        if (languageInDisk == null)
                        {
                            var deviceLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                            if (LanguageMap1.ContainsKey(deviceLanguage))
                            {
                                if (RegionInfo.CurrentRegion.EnglishName == "India")
                                    languageInDisk = "Hindi";
                                else
                                    languageInDisk = LanguageMap1[deviceLanguage];
                            }
                            else
                                languageInDisk = "All";
                            LocalCache.Instance.Set("MovieLanguage", languageInDisk);
                        }                        
                    }
                    catch(Exception)
                    {
                        languageInDisk = "All";
                    }
                    selectedLanguage = languageInDisk;
                }
                return selectedLanguage;
            }
            set
            {
                selectedLanguage = value;
                LocalCache.Instance.Set("MovieLanguage", selectedLanguage);
            }
        }

        public static List<int> Years = Enumerable.Range(1951, 75).ToList();
    }
}
