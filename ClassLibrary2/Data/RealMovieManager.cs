using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Trending;

namespace MovieBuffLib
{
    public class RealMovieManager : IMovieManager
    {
        List<Action<TmdbMovie>> actions = new List<Action<TmdbMovie>>
        {
            (item) => {item.ToiRating = new ToiProvider().GetData(item.Title);},
            (item) => {item.HtRating = new HtProvider().GetData(item.Title); },
            (item) => {item.ImdbRating = new ImdbProvider().GetData(item.ImdbId);}
        };

        public bool IsRefreshed { get; set; }
        public string LastRefresh { get; set; }
        public IMovieDataProvider dataProvider;
        public ILocalDataProvider LocalDataProvider;
        public Dictionary<string, TmdbMovie> MovieData = new Dictionary<string, TmdbMovie>();
        private List<TmdbMovie> upcoming;
        private List<TmdbMovie> nowPlaying;
        private List<TmdbMovie> trending;

        public RealMovieManager(ILocalDataProvider provider)
        {
            LocalDataProvider = provider;
            dataProvider = new TMdbDataProvider();
        }

        public void LoadInternalAsync()
        {
            TMDbClient client = new TMDbClient("c6b31d1cdad6a56a23f0c913e2482a31");
            upcoming = client.GetMovieUpcomingListAsync().Result.Results.Select(x => new TmdbMovie
            {
                Id = x.Id,
                Title = x.Title,
                PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
                BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
                Overview = x.Overview
            }).ToList();
            nowPlaying = client.GetMovieNowPlayingListAsync().Result.Results.Select(x => new TmdbMovie
            {
                Id = x.Id,
                Title = x.Title,
                PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
                BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
                Overview = x.Overview
            }).ToList();
            trending = client.GetTrendingMoviesAsync(TimeWindow.Week).Result.Results.Select(x => new TmdbMovie
            {
                Id = x.Id,
                Title = x.Title,
                PosterPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.PosterPath}",
                BackdropPath = $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{x.BackdropPath}",
                Overview = x.Overview
            }).ToList();
        }

        public void LoadInternalAsync1()
        {
            try
            {
                var data = LocalDataProvider.Get("MovieData");
                var lastRefresh = LocalDataProvider.Get("LastRefresh");

                if (!string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(lastRefresh))
                {
                    var lastRefreshDate = DateTime.Parse(lastRefresh);
                    var nextDay = lastRefreshDate.AddDays(1);
                    var midNight = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day);

                    //if (DateTime.Now.Subtract(lastRefreshDate).TotalHours <= 12 &&
                    //    (midNight > lastRefreshDate && midNight > DateTime.Now))
                    //    MovieData = JsonConvert.DeserializeObject<Dictionary<string, TmdbMovie>>(data);
                    //else
                    //{
                        RefreshData();
                        LocalDataProvider.Set("MovieData", JsonConvert.SerializeObject(MovieData));
                        LocalDataProvider.Set("LastRefresh", DateTime.Now.ToString());
                        IsRefreshed = true;
                        LastRefresh = DateTime.Now.ToString();
                    //}
                }
                else
                {
                    RefreshData();
                    LocalDataProvider.Set("MovieData", JsonConvert.SerializeObject(MovieData));
                    LocalDataProvider.Set("LastRefresh", DateTime.Now.ToString());
                    IsRefreshed = true;
                    LastRefresh = DateTime.Now.ToString();
                }
            }
            catch (Exception ex)
            {
                LastRefresh = ex.ToString();
            }
        }

        public async void RefreshData()
        {
            var currentTime = DateTime.Now;
            int CurrentDay = currentTime.Day;

            int CurrentYear = currentTime.Year;
            int CurrentMonth = currentTime.Month;
            int PreviousMonth = currentTime.AddMonths(-1).Month;
            int PreviousYear = currentTime.AddMonths(-1).Year;
            int NextMonth = currentTime.AddMonths(1).Month;
            int NextYear = currentTime.AddMonths(1).Year;
            for (int i = 1; i < 4; i++)
            {
                //Parallel.For(1, 4, (i) =>
                //{
                if (i == 1)
                    FillData(PreviousYear, PreviousMonth);
                else if (i == 2)
                    FillData(CurrentYear, CurrentMonth);
                else if (i == 3)
                    FillData(NextYear, NextMonth);
                //else if (i == 4)
                //    dataProvider.(2018, 2);
                }
        }
        public List<TmdbMovie> Upcoming => upcoming;
        public List<TmdbMovie> NowPlaying => nowPlaying;
        public List<TmdbMovie> Trending => trending;


        //{
        //    return upcoming;
        //}

        public List<TmdbMovie> GetReleased()
        {
            var newList = MovieData.Values.Where(c => c.ReleaseDate <= DateTime.Now).ToList();
            return newList.OrderByDescending(m => m.ReleaseDate).ToList();
        }

        public List<TmdbMovie> GetReleasedThisWeek()
        {
            var list = GetReleased();
            var latestReleaseData = GetReleased()[0].ReleaseDate;
            return list.Where(l => l.ReleaseDate == GetReleased()[0].ReleaseDate).ToList();
        }

        public List<TmdbMovie> GetUpcoming()
        {
            var list = MovieData.Values.Where(c => c.ReleaseDate > DateTime.Now).ToList();
            return list.OrderBy(m => m.ReleaseDate).ToList();
        }

        public List<TmdbMovie> GetUpcomingNextWeek()
        {
            var list = GetUpcoming();
            var latestReleaseData = list[0].ReleaseDate;
            return list.Where(l => l.ReleaseDate == latestReleaseData).ToList();
        }

        public List<TmdbMovie> GetAllTrailers()
        {
            var list = MovieData.Values.Where(c => !string.IsNullOrWhiteSpace(c.Trailer) && c.ReleaseDate > DateTime.Now).ToList();
            return list.OrderBy(m => m.ReleaseDate).ToList();
        }

        public TmdbMovie FindMovie(string movieName)
        {
            return MovieData.Values.Where(m => m.OriginalTitle == movieName).FirstOrDefault();
        }

        public bool IsMoviePresent(string movieName)
        {
            var movie = MovieData.Values.Where(m => m.OriginalTitle == movieName).FirstOrDefault();
            return movie != null;
        }

        public Credits GetCast(int movieId, string name)
        {
            return new Credits { Cast = new List<Cast>() };
            //var data = MovieData[GetKey(movieId, name)].Credits;
            //if (data == null)
            //    return new Credits { Cast = new List<Cast>() };
            //return data;
        }

        public string GetToiRating(string key) => MovieData[key].ToiRating;

        public string GetHtRating(string key) => MovieData[key].HtRating;

        public string GetImdbRating(string key) => MovieData[key].ImdbRating;

        public CastDetail GetMovieCredits(long castId) => new TMdbDataProvider().GetMovieCredits(castId);

        public TmdbMovie GetMovie(int movieId, string name) => MovieData[GetKey(movieId, name)];

        public string GetGenres(TmdbMovie movie, int count = 0)
        {
            var genres = movie.GenreText;
            if (count == 0 || string.IsNullOrWhiteSpace(genres))
                return genres;
            var splits = genres.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Take(count);
            var join = string.Join(", ", splits);
            return join;
        }

        public string GetFullOverview(int movieId, string name)
        {
            var movie = upcoming.Find(x => x.Id == movieId);
            if (movie != null) return movie.Overview;
            movie = nowPlaying.Find(x => x.Id == movieId);
            if (movie != null) return movie.Overview;
            movie = trending.Find(x => x.Id == movieId);
            if (movie != null) return movie.Overview;
            return name;
            //var movie = MovieData[GetKey(movieId, name)];
            //var data = $"{movie?.Overview}\n\nRuntime: {movie?.Runtime} minutes\n\nGenre: {movie?.GenreText}\n\nRelease Date: {movie?.ReleaseDate?.ToString("d MMM yyyy")}";
            //return data;
        }

        public string GetTrailer(int movieId, string name) => "";// MovieData[GetKey(movieId, name)].Trailer;

        public void FillData(int year, int month)
        {
            string key;
            var data = dataProvider.GetData(year, month);
            //foreach (var item in data)
            Parallel.ForEach(data, (item) =>
            {
                key = (item.Id == 0) ? item.Title.Replace(" ", "") : item.Id.ToString();
                item.ImdbRating = new ImdbProvider().GetData(item.ImdbId);
                //if (item.ReleaseDate <= DateTime.Now)
                //{
                //    Parallel.ForEach(actions, (action) =>
                //    {
                //        action(item);
                //    });
                //}
                MovieData.Add(GetKey((int)item.Id, item.Title), item);
            }
            );
        }

        public string GetKey(int id, string name) => id == 0 ? name.Replace(" ", "") : id.ToString();
    }
}