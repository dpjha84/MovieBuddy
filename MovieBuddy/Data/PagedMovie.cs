using System;
using System.Collections.Generic;

namespace MovieBuddy.Data
{
    public class PagedMovie
    {
        public static PagedMovie Instance = new PagedMovie();
        private int nowPlayingPage = 0, upcomingPage = 0, popularPage = 0, topRatedPage = 0;

        public void Reset(MovieListType type)
        {
            switch (type)
            {
                case MovieListType.NowPlaying:
                    nowPlayingPage = 0;
                    break;
                case MovieListType.Upcoming:
                    upcomingPage = 0;
                    break;
                case MovieListType.Popular:
                    popularPage = 0;
                    break;
                case MovieListType.TopRated:
                    topRatedPage = 0;
                    break;
                default:
                    throw new ArgumentException("Invalid Movie List Type");
            }
        }

        public List<TMDbLib.Objects.Search.SearchMovie> GetMovies(MovieListType type)
        {
            return type switch
            {
                MovieListType.NowPlaying => MovieManager.Instance.GetNowPlaying(++nowPlayingPage),

                MovieListType.Upcoming => MovieManager.Instance.GetUpcoming(++upcomingPage),

                MovieListType.Popular => MovieManager.Instance.GetPopular(++popularPage),

                MovieListType.TopRated => MovieManager.Instance.GetTopRated(++topRatedPage),

                _ => throw new ArgumentException("Invalid Movie List Type"),
            };
        }
    }
}