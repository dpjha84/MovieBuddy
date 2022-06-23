using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMDbLib.Objects.General;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    internal class MoviesWithTrailer
    {
        private readonly TClient tClient;
        private Func<TSMovie, bool> filterNowPlaying;
        private string nowPlayingBaseUrl;
        private readonly ConcurrentDictionary<string, int> totalPages = new ConcurrentDictionary<string, int>();
        public static MoviesWithTrailer Instance => _instance;
        private static readonly MoviesWithTrailer _instance = new MoviesWithTrailer();

        public MoviesWithTrailer()
        {
            tClient = new TClient();
        }

        List<TSMovie> nowPlaying = new List<TSMovie>();
        List<TSMovie> combinedList = new List<TSMovie>();
        List<TSMovie> upcoming = new List<TSMovie>();
        int start = 0, internalPage = 1;
        List<string> vids = new List<string>();
        Dictionary<int, List<string>> pagedData = new Dictionary<int, List<string>>();
        public List<string> Get(int page = 1)
        {
            //if (pagedData.ContainsKey(page)) return pagedData[page];
            if (page == 1)
            {
                internalPage = 1;
                start = 0;
            }

            var videos = new List<string>();
            while (videos.Count < 5)
            {
                if (start == 0)
                {
                    nowPlaying = NowPlayingMovies.Instance.TotalPages == 0 || internalPage <= NowPlayingMovies.Instance.TotalPages
                        ? NowPlayingMovies.Instance.Get(internalPage) ?? new List<TSMovie>()
                        : new List<TSMovie>();

                    upcoming = UpcomingMovies.Instance.TotalPages == 0 || internalPage <= UpcomingMovies.Instance.TotalPages
                        ? UpcomingMovies.Instance.Get(internalPage) ?? new List<TSMovie>()
                        : new List<TSMovie>();

                    combinedList = nowPlaying.Concat(upcoming).ToList();
                }
                if (internalPage > NowPlayingMovies.Instance.TotalPages &&
                       internalPage > UpcomingMovies.Instance.TotalPages)
                    break;
                videos.AddRange(LoadTrailers(combinedList, videos.Count));
            }
            //pagedData[page] = videos;
            return videos;

            //if (npPage <= ucPage)
            //{
            //    if (prevNpPage < npPage)
            //    {
            //        nowPlaying = NowPlayingMovies.Instance.Get(npPage);
            //        prevNpPage = npPage;
            //    }
            //    movies = nowPlaying;
            //}
            //else
            //{
            //    if (prevUcPage < ucPage)
            //    {
            //        upcoming = UpcomingMovies.Instance.Get(ucPage);
            //        prevUcPage = ucPage;
            //    }
            //    movies = upcoming;
            //}


            //var videos = LoadTrailers(nowPlaying, "np");
            //if (videos.Count < 5)
            //{
            //    upcoming = UpcomingMovies.Instance.Get(page);
            //    videos.AddRange(LoadTrailers(upcoming, "uc"));
            //}
            //return videos;
        }

        private List<string> LoadTrailers(List<TSMovie> movies, int existingCount)
        {
            var videos = new List<string>();
            if (movies == null)
                return videos;
            int i;
            for (i = start; i < movies.Count; i++)
            {
                var videos1 = tClient.GetMovieVideosAsync(movies[i].Id).Result.Results;
                if (videos1 == null || videos1.Count == 0) continue;
                foreach (var item in videos1)
                {
                    if (!string.IsNullOrWhiteSpace(item.Key)
                        && !string.IsNullOrWhiteSpace(item.Type)
                        && (item.Type.Equals("Trailer", StringComparison.InvariantCultureIgnoreCase) || item.Type.Equals("Teaser", StringComparison.InvariantCultureIgnoreCase))
                        && item.Key.IsValidVideo()
                        )
                    {
                        videos.Add(item.Key);
                        break;
                    }
                }
                if (videos.Count + existingCount >= 5)
                {
                    //if (i == movies.Count - 1)
                    //{
                    //    npStart = ucStart = 0;
                    //    if (type == "np")
                    //        npPage++;
                    //    else
                    //        ucPage++;
                    //}                        
                    //else
                    //    npStart = ucStart = i+1;
                    break;
                }
            }
            if (i == movies.Count)
            {
                internalPage++;
                start = 0;
            }
            else
                start = i+1;
            return videos;
        }
    }
}