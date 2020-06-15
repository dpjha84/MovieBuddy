using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using TMDbLib.Client;
using TMDbLib.Objects.General;

namespace MovieBuddy
{
    public class LocalCache
    {
        public const string GenresKey = "GenreList";
        public const string LanguagesKey = "LanguageList";
        const string Prefs = "MySharedPrefs";

        public static LocalCache Instance { get; } = new LocalCache();
        public void Reset()
        {
            SharedPref.Edit().Clear().Commit();
        }

        public string Get(string key) => SharedPref.GetString(key, null);

        public void Set(string key, string value)
        {
            var prefEditor = SharedPref.Edit();
            var data = value ?? "";
            prefEditor.PutString(key.ToString(), data);
            prefEditor.Commit();
        }

        ISharedPreferences SharedPref { get; } = Application.Context.GetSharedPreferences(Prefs, FileCreationMode.Private);
    }

    //public class PagedDataProvider
    //{
    //    public int page = 1;

    //    public int totalPages = 0;
    //    public List<TMDbLib.Objects.Search.SearchMovie> GetData(Func<string> getRequestUrl, Func<List<TMDbLib.Objects.Search.SearchMovie>> getForAll)
    //    {
    //        if (page == 1) totalPages = int.MaxValue;
    //        if (page > totalPages) return null;
    //        if (Globals.SelectedLanguage == "All") return getForAll();

    //        using (var client = new HttpClient())
    //        {
    //            var response = client.GetAsync(getRequestUrl());
    //            if (response.Result.IsSuccessStatusCode)
    //            {
    //                var data = response.Result.Content.ReadAsStringAsync();
    //                var res = JsonConvert.DeserializeObject<SearchContainer<TMDbLib.Objects.Search.SearchMovie>>(data.Result);
    //                totalPages = res.TotalPages;
    //                return res.Results.Where(x => x.ReleaseDate >= DateTime.Parse(startDate)).ToList();
    //            }
    //        }
    //        return null;
    //    }
    //}

    public class PagedFetcher<T> : IEnumerable<Page<T>>, IEnumerator<Page<T>>
    {
        int skip = 0;
        int pageSize = 3;
        int totalItems = int.MaxValue;
        readonly TMDbLib.Client.TMDbClient client;
        Func<T> webDataFetcher;
        private Page<T> currentPage;

        public PagedFetcher(TMDbLib.Client.TMDbClient client, Func<T> getWebData)
        {
            this.client = client;
            webDataFetcher = getWebData;
        }

        public Page<T> Current
        {
            get
            {
                return currentPage;
            }
        }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
        }

        public IEnumerator<Page<T>> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (skip > totalItems) return false;
            var data = webDataFetcher();
            currentPage = new Page<T> { Data = data };
            skip += pageSize;
            return true;
        }

        public void Reset()
        {
            skip = 0;
            currentPage = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }

    public class Page<T>
    {
        public int Id;
        public T Data;
    }
}