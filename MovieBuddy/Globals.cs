using Android.App;
using Android.Content;
using System.Collections.Generic;
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
                    .SetPositiveButton("RETRY", (sender, args) => { 
                    
                    })
                    .Show();
            return;
            }
        }
    }
}
