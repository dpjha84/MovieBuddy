using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    public static class TmdbExtensions
    {
        public static SearchContainer<TMovie> GetMoviesByUrl(this TMDbClient tmdbClient, string url, int page)
        {
            url += $"page={page}&with_original_language={Globals.LanguageMap[Globals.SelectedLanguage]}";
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url);
                if (response.Result.IsSuccessStatusCode)
                {
                    var data = response.Result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<SearchContainer<TMovie>>(data.Result);
                }
            }
            return null;
        }
    }
}