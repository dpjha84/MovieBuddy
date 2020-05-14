using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuffLib
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
        bool IsMoviePresent(string movieName);
        void LoadInternalAsync();
        void RefreshData();
        bool IsRefreshed { get; set; }
        string LastRefresh { get; set; }
        string GetGenres(TmdbMovie movie, int count = 0);
    }
}
