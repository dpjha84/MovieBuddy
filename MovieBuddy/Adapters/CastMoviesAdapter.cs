using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class CastMoviesAdapter : MoviesAdapter
    {
        protected override string GetExtraText(TSearchMovie movie) => movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.Year.ToString() : "";
    }
}