using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Android.Support.V7.Widget;
using Android.Views;

namespace MovieBuddy
{
    public class MoviesFragment : RecyclerViewFragment
    {

        ClickableWithPagingAdapter<TMDbLib.Objects.Search.SearchMovie> movieAdapter;
        public MovieListType MovieListType { get { return (MovieListType)Arguments.GetInt("movieListType"); } }

        public static MoviesFragment NewInstance(MovieListType type)
        {
            var frag1 = new MoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieListType", (int)type);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            PagedMovie.Instance.Reset(MovieListType);
            movieAdapter = new MoviesAdapter();
            movieAdapter.ItemClick += OnItemClick;
            return movieAdapter;
        }

        protected override void SetupOnScroll()
        {
            var onScrollListener = new XamarinRecyclerViewOnScrollListener();
            onScrollListener.LoadMoreEvent += (object sender, EventArgs e) => {
                GetData();
            };
            nsv.SetOnScrollChangeListener(onScrollListener);
        }

        protected override void GetData()
        {
            var data = GetMovies();
            if (data == null) return;
            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            movieAdapter.LoadData(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        }

        protected virtual List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return PagedMovie.Instance.GetMovies(MovieListType);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        public class PagedMovie
        {
            public void Reset(MovieListType type)
            {
                if(type == MovieListType.NowPlaying)
                    nowPlayingPage = 0;

                if (type == MovieListType.Upcoming)
                    upcomingPage = 0;

                if (type == MovieListType.Popular)
                    popularPage = 0;
                if (type == MovieListType.TopRated)
                    topRatedPage = 0;
            }
            public static PagedMovie Instance = new PagedMovie();
            private static int nowPlayingPage = 0, upcomingPage = 0, popularPage = 0, topRatedPage = 0;
            public List<TMDbLib.Objects.Search.SearchMovie> GetMovies(MovieListType type)
            {
                switch (type)
                {
                    case MovieListType.NowPlaying:
                        nowPlayingPage++;
                        return MovieManager.Instance.GetNowPlaying(nowPlayingPage);
                    case MovieListType.Upcoming:
                        upcomingPage++;
                        return MovieManager.Instance.GetUpcoming(upcomingPage);
                    case MovieListType.Popular:
                        popularPage++;
                        return MovieManager.Instance.GetPopular(popularPage);
                    case MovieListType.TopRated:
                        topRatedPage++;
                        return MovieManager.Instance.GetTopRated(topRatedPage);
                    default:
                        return null;
                }
                //return type switch
                //{
                //    MovieListType.NowPlaying => MovieManager.Instance.NowPlaying,
                //    MovieListType.Upcoming => MovieManager.Instance.Upcoming,
                //    MovieListType.Popular => MovieManager.Instance.Popular,
                //    MovieListType.TopRated => MovieManager.Instance.TopRated,
                //    _ => throw new ArgumentException("Invalid Movie List Type"),
                //};
            }
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

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            IMenuItem filter = menu.FindItem(Resource.Id.menu_info);
            if (filter != null)
            {
                if (MovieListType == MovieListType.NowPlaying || MovieListType == MovieListType.Upcoming)
                    filter.SetVisible(true);
                else
                    filter.SetVisible(false);
            }
            base.OnPrepareOptionsMenu(menu);
        }

        public override void OnDestroy()
        {
            if (movieAdapter != null)
                movieAdapter.ItemClick -= OnItemClick;
            base.OnDestroy();
        }

        //public override void OnStop()
        //{
        //    foreach (var imageView in adapter.ImageViewsToClean)
        //    {
        //        Helper.Clear(Context, imageView);
        //    }
        //    base.OnStop();
        //}

        //public override void OnResume()
        //{
        //    adapter = new MoviesAdapter(GetMovies());
        //    //foreach (var imageView in adapter.moviesImageViews)
        //    //{
        //    //    Helper.SetImage(Context, imageView);
        //    //}
        //    base.OnResume();
        //}
    }

    public enum MovieListType
    {
        NowPlaying,
        Upcoming,
        Popular,
        TopRated
    }
}