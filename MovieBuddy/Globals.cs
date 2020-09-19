using Android.App;
using Android.Content;
using Newtonsoft.Json;
using System.Collections.Generic;
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

        static List<int> starredMovies = null;

        public static void AddToStarredMovies(int movieId)
        {
            StarredMovies.Add(movieId);
            StarredMovies = starredMovies;
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

        static string selectedLanguage = null;
        public static string SelectedLanguage
        {
            get
            {
                if (selectedLanguage == null)
                {
                    var languageInDisk = LocalCache.Instance.Get("MovieLanguage");
                    if (languageInDisk == null)
                    {
                        languageInDisk = "All";
                        LocalCache.Instance.Set("MovieLanguage", languageInDisk);
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

        public static void CheckInternet(Context context)
        {
            var current = Connectivity.NetworkAccess;

            switch (current)
            {
                case NetworkAccess.None:
                case NetworkAccess.Unknown:
                    new AlertDialog.Builder(context)
                    .SetTitle("Delete entry")
                    .SetMessage("No Internet")
                    .SetPositiveButton("RETRY", (sender, args) =>
                    {

                    })
                    .Show();
                    return;
            }
        }

        public static List<int> Years = Enumerable.Range(1951, 75).ToList();
    }
}
