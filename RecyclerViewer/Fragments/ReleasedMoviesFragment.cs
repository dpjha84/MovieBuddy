using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using MovieBuffLib;
using Android.Widget;
using RecyclerViewer;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace RecyclerViewer
{    
    public enum MovieListType
    {
        NowPlaying,
        Upcoming,
        Trending
    }
    public class MoviesFragment : BaseFragment
    {
        MovieGridAdapter adapter;
        protected Func<List<TmdbMovie>> moviesToShow;

        //public override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);
        //}
        private MovieListType movieListType;// = MovieListType.NowPlaying;
        public static MoviesFragment NewInstance(bool isReleased)
        {
            var frag1 = new MoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutBoolean("isReleased", isReleased);
            frag1.Arguments = bundle;
            return frag1;// new MoviesFragment { Arguments = new Bundle(), moviesToShow = movies };
        }

        public static MoviesFragment NewInstance(MovieListType type)
        {
            var frag1 = new MoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieListType", (int)type);
            frag1.Arguments = bundle;
            return frag1;// new MoviesFragment { Arguments = new Bundle(), moviesToShow = movies };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                movieListType = (MovieListType)Arguments.GetInt("movieListType");
                View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

                RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
                rv.HasFixedSize = true;

                var llm = new GridLayoutManager(this.Context, 3, GridLayoutManager.Vertical, false);
                rv.SetLayoutManager(llm);

                //var awrPosts = new List<TmdbMovie>();
                //adapter = new MovieGridAdapter(isReleased ? MovieManager.Instance.GetReleased() : MovieManager.Instance.GetUpcoming(), true);
                adapter = new MovieGridAdapter(GetMovies(), true);
                //adapter = new MovieGridAdapter(awrPosts, true);
                adapter.ItemClick += OnItemClick;
                rv.SetAdapter(adapter);

                //new retrievePosts(Context, adapter).Execute();
                return rootView;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        List<TmdbMovie> GetMovies()
        {
            switch(movieListType)
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


    public class retrievePosts : AsyncTask<string, string, List<TmdbMovie>>
    {
        List<TmdbMovie> awr;
        Context mContext;
        MovieGridAdapter mAdapter;
        public retrievePosts(Context context, MovieGridAdapter adapter)
        {
            mContext = context;
            mAdapter = adapter;
            awr = new List<TmdbMovie>();
        }

        protected override void OnPreExecute()
        {
            //AndroidHUD.AndHUD.Shared.ShowImage(mContext, Resource.Drawable.load2, "Getting Posts...");
        }

        protected override List<TmdbMovie> RunInBackground(params string[] @params)
        {
            for (int i = 0; i < 4; i++)
            {
                awr.Add(MovieManager.Instance.GetReleased()[i]);
                Thread.Sleep(2000);
            }
            return awr;
        }

        protected override void OnPostExecute(List<TmdbMovie> result)// this function is supposed to run on the UI thread
        {
            base.OnPostExecute(result);
            mAdapter = new MovieGridAdapter(result, true); // assigning the data here
            mAdapter.NotifyDataSetChanged();//y'all kn what i'm trying to do here
            //AndroidHUD.AndHUD.Shared.Dismiss(mContext);
            Toast.MakeText(mContext, "successful", ToastLength.Long).Show();
        }
    }
}