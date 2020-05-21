using Android.OS;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class SimilarMoviesFragment : MoviesFragment
    {
        public static SimilarMoviesFragment NewInstance(int movieId)
        {
            var frag1 = new SimilarMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieManager.Instance.GetSimilar(MovieId);
        }
    }
}