using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;

namespace MovieBuddy
{
    public class TMdbDataProvider : IMovieDataProvider
    {
        public QueryMovie FindMovie(string movieName)
        {
            var json = CallRestMethod($"https://api.themoviedb.org/3/search/movie?query={movieName}&api_key=967abbed236b6e3d97da4ced26b2199c");
            var movie = QueryMovie.FromJson(json.Result);

            return movie;
        }

        public List<TmdbMovie> GetData(int year, int month)
        {
            List<TmdbMovie> output = new List<TmdbMovie>();
            int page = 1;
            var startDate = new DateTime(year, month, 1);
            string startDay = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            var endDay = startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            while (true)
            {
                string details = CallRestMethod($"https://api.themoviedb.org/3/discover/movie?api_key=967abbed236b6e3d97da4ced26b2199c&release_date.gte={startDay}&release_date.lte={endDay}&page={page++}").Result;
                var data = JsonConvert.DeserializeObject<SearchContainerWithDates<TmdbMovie>>(details);
                output.AddRange(data.Results.Where(m => m.OriginalLanguage == "hi" && m.ReleaseDate.Value.Year == year && m.ReleaseDate.Value.Month == month));
                if (page > data.TotalPages)
                    break;
            }

            return output;
        }

        private static int count = 0;
        private static DateTime? firstTmDbCall = null;

        class Timer
        {
            public int count { get; set; }
            public DateTime? Time { get; set; }
        }

        static ConcurrentDictionary<int, DateTime?> timers = new ConcurrentDictionary<int, DateTime?>();
        static object lockObj = new object();
        public async Task<string> CallRestMethod(string url)
        {
            lock (lockObj)
            {
                count++;
                timers.AddOrUpdate(count, DateTime.Now, (i1, time) => time);
            }
            using (var client = new HttpClient())
            {
                try
                {
                    if (count > 30)
                    {
                        DateTime? timeValue;
                        if (timers.TryGetValue(count - 30, out timeValue))
                        {
                            var diff = DateTime.Now.Subtract(timeValue.Value).TotalMilliseconds;
                            if (diff < 10000)
                            {
                                //Debug.WriteLine($"{DateTime.Now} count: {count} Sleeping {url}");
                                Thread.Sleep(10000 - (int)diff);
                                //Debug.WriteLine($"{DateTime.Now} count: {count} Slept {url}");
                            }
                        }
                    }
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();//ReadAsAsync<Product>();
                    }
                    //else
                    //{
                    //    Debug.WriteLine($"Error - {DateTime.Now} count: {count} {url} : {response.StatusCode}");
                    //}
                    //return product;
                    //var json = httpClient.GetStringAsync(url);
                    //var result = json.Result;
                    //return result;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }

        public CastDetail GetCast(int movieId)
        {
            var json = CallRestMethod($"https://api.themoviedb.org/3/movie/{movieId}/casts?api_key=967abbed236b6e3d97da4ced26b2199c");
            return CastDetail.FromJson(json.Result);
        }

        //public string GetTrailer(int movieId)
        //{
        //    var data = CallRestMethod($"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key=967abbed236b6e3d97da4ced26b2199c&append_to_response=videos,credits");
        //    //var data = CallRestMethod($"https://api.themoviedb.org/3/movie/47964?api_key=967abbed236b6e3d97da4ced26b2199c&append_to_response=videos,credits");
        //    var trailer = Trailer.FromJson(data);
        //    //return "";
        //    return trailer.Results != null && trailer.Results.Count > 0 ? trailer.Results[0].Key : null;
        //}

        public CastDetail GetMovieCredits(long castId)
        {
            var data = CallRestMethod($"https://api.themoviedb.org/3/person/{castId}/movie_credits?api_key=967abbed236b6e3d97da4ced26b2199c");
            return CastDetail.FromJson(data.Result);
        }

        public TmdbMovie GetMovie(long movieId)
        {
            var data = CallRestMethod($"https://api.themoviedb.org/3/movie/{movieId}?api_key=967abbed236b6e3d97da4ced26b2199c&append_to_response=videos,credits");
            return TmdbMovie.FromJson(data.Result);
        }
    }
}