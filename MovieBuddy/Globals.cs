using System.Collections.Generic;

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
            { "Portuguese", "pt" },
            { "Russian", "ru" },
            { "Spanish", "es" },
            { "Tamil", "ta" },
            { "Telugu", "te" },
            { "Turkish", "tr" }
        };

        static string selectedLanguage = null;
        public static string SelectedLanguage
        {
            get
            {
                if (selectedLanguage == null)
                {
                    var languageInDisk = LocalDataProvider.Instance.Get("MovieLanguage");
                    if (languageInDisk == null)
                    {
                        languageInDisk = "All";
                        LocalDataProvider.Instance.Set("MovieLanguage", languageInDisk);
                    }
                    selectedLanguage = languageInDisk;
                }
                return selectedLanguage;
            }
            set
            {
                selectedLanguage = value;
                LocalDataProvider.Instance.Set("MovieLanguage", selectedLanguage);
            }
        }
    }
}
