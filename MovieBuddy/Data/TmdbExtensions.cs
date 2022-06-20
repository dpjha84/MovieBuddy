using Newtonsoft.Json;
using System.Net.Http;
using TMDbLib.Objects.General;
using TMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    public static class TmdbExtensions
    {
        public static SearchContainer<TMovie> GetMoviesByUrl(this TClient tmdbClient, string url, int page)
        {
            TClientBase.calls++;
            url += $"page={page}&with_original_language={Globals.LanguageMap[Globals.Language]}";
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

        public static bool IsValidVideo(this string videoId)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"https://www.youtube.com/oembed?url=https://www.youtube.com/watch?v={videoId}");
                if (response.Result.IsSuccessStatusCode)
                {
                    //var data = response.Result.Content.ReadAsStringAsync();
                    return true;// JsonConvert.DeserializeObject<SearchContainer<TMovie>>(data.Result);
                }
            }
            return false;
        }
    }
}