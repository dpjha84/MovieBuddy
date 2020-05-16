using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;

namespace MovieBuddy
{
    public class LocalDataProvider : ILocalDataProvider
    {
        public void Reset()
        {
            var prefs = Application.Context.GetSharedPreferences("MySharedPrefs", FileCreationMode.Private);
            prefs.Edit().Clear().Commit();
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