using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
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

        int page = 1;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieManager.Instance.GetSimilar(MovieId, page++);
        }

        protected override void ResetPages() { }
    }

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

        int page = 1;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieManager.Instance.SearchMovie(Query, page++);
        }
    }

    public class SearchPersonFragment : CastFragment
    {
        protected SearchPersonAdapter searchPersonAdapter;

        public static SearchPersonFragment NewInstance(string query)
        {
            var frag1 = new SearchPersonFragment();
            Bundle bundle = new Bundle();
            bundle.PutString("query", query);
            frag1.Arguments = bundle;
            return frag1;
        }

        int page = 1;
        protected override void GetData()
        {
            var data = MovieManager.Instance.SearchPerson(Query, page++);
            if (data == null) return;
            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            searchPersonAdapter.LoadData(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            searchPersonAdapter = new SearchPersonAdapter();
            searchPersonAdapter.ItemClick += OnItemClick;
            return searchPersonAdapter;
        }

        protected override void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("castId", (sender as SearchPersonAdapter).Cast[position].Id);
            b.PutString("castName", (sender as SearchPersonAdapter).Cast[position].Name);
            var cast = (sender as SearchPersonAdapter).Cast[position];
            var backdrop = cast.ProfilePath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
        }
    }
}