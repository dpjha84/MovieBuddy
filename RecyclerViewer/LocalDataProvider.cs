using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Android.App;
using Android.Content;
using MovieBuffLib;

namespace RecyclerViewer
{
    public class LocalDataProvider : ILocalDataProvider
    {
        public static Movie[] PreviousMonthMovies { get; set; }
        public static Movie[] CurrentMonthMovies { get; set; }
        public static Movie[] NextMonthMovies { get; set; }
        public static int PreviousMonth { get; set; }
        public static int CurrentMonth { get; set; }
        public static int NextMonth { get; set; }
        public static int PreviousMonthYear { get; set; }
        public static int CurrentMonthYear { get; set; }
        public static int NextMonthYear { get; set; }

        public void Reset()
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            prefs.Edit().Clear().Commit();
            //prefs.Edit().Apply();  
            //Set("201710", null);
            //Set("201711", null);
            //Set("201712", null);
            //Set("201712", null);
        }

        public string Get(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            if (prefs.Contains(key))
            {
                return prefs.GetString(key, null);
            }
            return null;
        }

        public void Set(string key, string value)
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            var prefEditor = prefs.Edit();

            var data = value != null ? value : "";
            prefEditor.PutString(key.ToString(), data);
            prefEditor.Commit();
        }
    }
}