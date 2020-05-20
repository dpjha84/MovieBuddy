using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace MovieBuddy
{    
    public enum MovieListType
    {
        NowPlaying,
        Upcoming,
        Trending,
        Popular,
        TopRated
    }

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
            movieId = Arguments.GetInt("movieId");
            return MovieManager.Instance.GetSimilar(movieId);
        }
    }

    public class MoviesFragment : BaseFragment
    {
        MoviesAdapter adapter;
        private MovieListType movieListType;

        public static MoviesFragment NewInstance(MovieListType type)
        {
            var frag1 = new MoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieListType", (int)type);
            frag1.Arguments = bundle;
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

                RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
                rv.NestedScrollingEnabled = false;
                rv.HasFixedSize = true;

                var llm = new GridLayoutManager(this.Context, 3, GridLayoutManager.Vertical, false);
                rv.SetLayoutManager(llm);

                adapter = new MoviesAdapter(GetMovies());
                adapter.ItemClick += OnItemClick;
                rv.SetAdapter(adapter);
                return rootView;
            }
            catch (Exception)
            {
            }
            return null;
        }

        protected virtual List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            movieListType = (MovieListType)Arguments.GetInt("movieListType");
            return movieListType switch
            {
                MovieListType.NowPlaying => MovieManager.Instance.NowPlaying,
                MovieListType.Upcoming => MovieManager.Instance.Upcoming,
                MovieListType.Trending => MovieManager.Instance.Trending,
                MovieListType.Popular => MovieManager.Instance.Popular,
                MovieListType.TopRated => MovieManager.Instance.TopRated,
                _ => throw new ArgumentException("Invalid Movie List Type"),
            };
        }

        protected override void OnItemClick(object sender, int position)
        {
            try
            {
                var movie = (sender as MoviesAdapter).movies[position];
                Intent intent = new Intent(this.Context, typeof(MovieInfoActivity));
                Bundle b = new Bundle();
                b.PutInt("movieId", movie.Id);
                b.PutString("movieName", movie.Title);
                var backdrop = movie.BackdropPath;
                b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
                intent.PutExtras(b);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, ex.ToString(), ToastLength.Long).Show();
            }            
        }

        public override void OnDestroy()
        {
            if(adapter != null)
                adapter.ItemClick -= OnItemClick;
            base.OnDestroy();
        }
    }
}