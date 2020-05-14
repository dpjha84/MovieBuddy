using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using TMDbLib.Objects.Search;
using System.Net;
using System.IO;
using TMDbLib.Objects.General;
using System.Text;
using Newtonsoft.Json;
using RecyclerViewer.Data;

namespace RecyclerViewer
{ 
    public class TMdbDataProvider : IMovieDataProvider
    {
        public QueryMovie FindMovie(string movieName)
        {
            var json = CallRestMethod($"https://api.themoviedb.org/3/search/movie?query={movieName}&api_key=967abbed236b6e3d97da4ced26b2199c");
            var movie = QueryMovie.FromJson(json);
            
            return movie;
        }
        public List<SearchMovie> GetData(int year, int month)
        {
            List<SearchMovie> output = new List<SearchMovie>();
            int page = 1;
            var startDate = new DateTime(year, month, 1);
            string startDay = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            var endDay = startDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            while (true)
            {
                string details = CallRestMethod($"https://api.themoviedb.org/3/discover/movie?api_key=967abbed236b6e3d97da4ced26b2199c&release_date.gte={startDay}&release_date.lte={endDay}&page={page++}");
                var data = JsonConvert.DeserializeObject<SearchContainerWithDates<SearchMovie>>(details);
                output.AddRange(data.Results.Where(m => m.OriginalLanguage == "hi" && m.ReleaseDate.Value.Year == year && m.ReleaseDate.Value.Month == month));
                if (page > data.TotalPages)
                    break;
            }

            return output;
        }

        public string CallRestMethod(string url)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            result = responseStream.ReadToEnd();
            webresponse.Close();
            return result;
        }

        public CastDetail GetCast(int movieId)
        {
            var json = CallRestMethod($"https://api.themoviedb.org/3/movie/{movieId}/casts?api_key=967abbed236b6e3d97da4ced26b2199c");
            return CastDetail.FromJson(json);
        }

        public string GetTrailer(int movieId)
        {
            var data = CallRestMethod($"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key=967abbed236b6e3d97da4ced26b2199c");
            var trailer = Trailer.FromJson(data);
            return trailer.Results != null && trailer.Results.Count > 0 ? trailer.Results[0].Key : null;
        }

        public CastDetail GetMovieCredits(long castId)
        {
            var data = CallRestMethod($"https://api.themoviedb.org/3/person/{castId}/movie_credits?api_key=967abbed236b6e3d97da4ced26b2199c");
            return CastDetail.FromJson(data);
        }

        public TmdbMovie GetMovie(long movieId)
        {
            var data = CallRestMethod($"https://api.themoviedb.org/3/movie/{movieId}?api_key=967abbed236b6e3d97da4ced26b2199c");
            return TmdbMovie.FromJson(data);
        }
    }
}