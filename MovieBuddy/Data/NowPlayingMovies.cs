using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMDbLib.Objects.General;
using TSMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy.Data
{
    internal class NowPlayingMovies
    {
        private readonly TClient tClient;
        private readonly Func<TSMovie, bool> filterNowPlaying;
        private readonly string nowPlayingBaseUrl;
        private readonly ConcurrentDictionary<string, int> totalPages = new ConcurrentDictionary<string, int>();
        public static NowPlayingMovies Instance => _instance;
        private static readonly NowPlayingMovies _instance = new NowPlayingMovies();

        public NowPlayingMovies()
        {
            tClient = new TClient();
            var today = DateTime.Now;
            var endDate = today.ToString("yyyy-MM-dd");
            var startDate = today.Subtract(TimeSpan.FromDays(30)).ToString("yyyy-MM-dd");
            nowPlayingBaseUrl = BuildUrl(startDate, "release_date.desc", endDate);
            filterNowPlaying = x => x.ReleaseDate >= DateTime.Parse(startDate)
                && x.ReleaseDate <= DateTime.Parse(endDate)
                && x.GenreIds?.Count > 0
                && x.PosterPath != null;
        }

        public int TotalPages
        {
            get
            {
                if (!totalPages.ContainsKey(Globals.Language))
                    return 0;
                return totalPages[Globals.Language];
            }
        }

        public List<TSMovie> Get(int page = 1)
        {
            if (page == 1)
                totalPages[Globals.Language] = int.MaxValue;

            if (totalPages.ContainsKey(Globals.Language) && page > totalPages[Globals.Language])
                return null;

            return CacheRepo.NowPlaying.GetOrCreate($"NowPlaying_{Globals.Language}_{page}", () =>
            {
                var result = new SearchContainer<TSMovie>();
                if (Globals.Language == "All")
                {
                    result = tClient.GetMovieNowPlayingListAsync(null, page).Result;
                    totalPages[Globals.Language] = result.TotalPages;
                    return result.Results;
                }
                else
                {
                    result = tClient.GetMoviesByUrl(nowPlayingBaseUrl, page);
                    totalPages[Globals.Language] = result.TotalPages;
                    return result.Results.Where(filterNowPlaying).ToList();
                }
            });
        }

        private string BuildUrl(string startDate, string sortBy, string endDate = null)
        {
            var sb = new StringBuilder();
            sb.Append("https://api.themoviedb.org/3/discover/movie?");
            sb.Append($"api_key={TClient.tmdbApiKey}&");
            sb.Append($"release_date.gte={startDate}&");
            sb.Append($"sort_by={sortBy}&");
            if (endDate != null)
                sb.Append($"release_date.lte={endDate}&");
            return sb.ToString();
        }
    }
}