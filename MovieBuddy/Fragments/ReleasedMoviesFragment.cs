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
        Trending
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
        MovieGridAdapter adapter;
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
                rv.HasFixedSize = true;

                var llm = new GridLayoutManager(this.Context, 3, GridLayoutManager.Vertical, false);
                rv.SetLayoutManager(llm);

                adapter = new MovieGridAdapter(GetMovies(), true);
                adapter.ItemClick += OnItemClick;
                rv.SetAdapter(adapter);
                return rootView;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        protected virtual List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            movieListType = (MovieListType)Arguments.GetInt("movieListType");
            switch (movieListType)
            {
                case MovieListType.NowPlaying:
                    return MovieManager.Instance.NowPlaying;
                case MovieListType.Upcoming:
                    return MovieManager.Instance.Upcoming;
                case MovieListType.Trending:
                    return MovieManager.Instance.Trending;
                default:
                    throw new ArgumentException("Invalid Movie List Type");
            }
        }

        protected override void OnItemClick(object sender, int position)
        {
            var movie = (sender as MovieGridAdapter).mPhotoAlbum[position];
            Intent intent = new Intent(this.Context, typeof(MovieInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("movieId", (int)movie.Id);
            b.PutString("movieName", movie.Title);
            var backdrop = movie.BackdropPath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
            intent.PutExtras(b);
            StartActivity(intent);
        }
    }
}