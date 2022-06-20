using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class ImdbMoviesAdapter : MoviesAdapter
    {
        protected override string GetExtraText(TSearchMovie movie) => movie.OriginalTitle;
    }
}