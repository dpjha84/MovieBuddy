using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class TmdbTopRatedMoviesAdapter : MoviesAdapter
    {
        protected override string GetExtraText(TSearchMovie movie) => $"{movie.VoteAverage * 10}%";
    }
}