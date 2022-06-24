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
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    internal class MoviesWithTrailer
    {
        private readonly TClient tClient;
        private readonly ConcurrentDictionary<string, int> totalPages = new ConcurrentDictionary<string, int>();
        public static MoviesWithTrailer Instance => _instance;
        private static readonly MoviesWithTrailer _instance = new MoviesWithTrailer();
        private bool done;
        public MoviesWithTrailer()
        {
            tClient = new TClient();
            MainActivity_MovieLanguageChanged(null, null);
            MainActivity.MovieLanguageChanged += MainActivity_MovieLanguageChanged;
        }

        Thread trailerMaintenanceThread = null;
        private void MainActivity_MovieLanguageChanged(object sender, EventArgs e)
        {
            while (trailerMaintenanceThread != null && trailerMaintenanceThread.IsAlive)
            {
                trailerMaintenanceThread.Abort();
                trailerMaintenanceThread = null;
            }
            trailerMaintenanceThread = new Thread(() =>
            {
                try
                {
                    done = false;
                    var language = Globals.Language;
                    int page = 1;
                    while (true)
                    {
                        var key = $"trailers_{language}_{page}";
                        var data = CacheRepo.Videos.GetOrCreate(key, () =>
                        {
                            var videos = GetData(page);
                            return videos ?? null;
                        });
                        if (data?.Count == 0)
                        {
                            done = true;
                            break;
                        }
                        page++;
                    }
                }
                catch (Exception ex)
                {
                }
            });
            trailerMaintenanceThread.Start();
        }

        List<TSMovie> nowPlaying = new List<TSMovie>();
        List<TSMovie> combinedList = new List<TSMovie>();
        List<TSMovie> upcoming = new List<TSMovie>();
        int start = 0, internalPage = 1;
        public List<string> Get(int page = 1)
        {
            int count = page == 1 ? 10 : 5;
            var key = $"trailers_{Globals.Language}_{page}";
            while (!CacheRepo.Videos.ContainsKey(key) && !done)
            {
                if (count <= 0)
                    return null;
                Thread.Sleep(200);
                count--;
            }
            return !CacheRepo.Videos.ContainsKey(key) ? null :
                CacheRepo.Videos.GetOrCreate(key, () =>
                {
                    var videos = GetData(page);
                    return videos ?? null;
                });
        }

        private List<string> GetData(int page)
        {
            if(page == 1)
            {
                start = 0;
                internalPage = 1;
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
            return videos;
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
                    break;
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