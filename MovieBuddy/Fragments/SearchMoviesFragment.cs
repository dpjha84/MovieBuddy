using Android.OS;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class SearchMoviesFragment : MoviesFragment
    {
        public static SearchMoviesFragment NewInstance(string query)
        {
            var frag1 = new SearchMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutString("query", query);
            frag1.Arguments = bundle;
            return frag1;
        }

        private int page = 1;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieManager.Instance.SearchMovie(Query, page++);
        }
    }
}