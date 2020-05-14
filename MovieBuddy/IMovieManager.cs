using System.Collections.Generic;

namespace MovieBuddy
{
    public interface IMovieManager
    {
        string GetFullOverview(int movieId, string name);

        CastDetail GetMovieCredits(long castId);

        Credits GetCast(int movieId, string name);

        List<TmdbMovie> GetReleased();

        List<TmdbMovie> GetAllTrailers();

        List<TmdbMovie> GetReleasedThisWeek();

        List<TmdbMovie> GetUpcomingNextWeek();

        TmdbMovie GetMovie(int movieId, string name);

        string GetToiRating(string key);

        string GetHtRating(string key);

        string GetImdbRating(string key);

        string GetTrailer(int movieId, string name);

        List<TmdbMovie> GetUpcoming();

        List<TmdbMovie> Upcoming { get; }
        List<TmdbMovie> NowPlaying { get; }
        List<TmdbMovie> Trending { get; }

        bool IsMoviePresent(string movieName);

        void LoadInternalAsync();

        void RefreshData();

        bool IsRefreshed { get; set; }
        string LastRefresh { get; set; }

        string GetGenres(TmdbMovie movie, int count = 0);
    }
}