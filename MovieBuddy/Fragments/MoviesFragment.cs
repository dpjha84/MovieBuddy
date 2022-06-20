using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MovieBuddy.Data;
using System;
using System.Collections.Generic;
using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class ImdbMoviesFragment : MoviesFragment
    {
        public static MoviesFragment NewInstance()
        {
            var frag1 = new ImdbMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieListType", (int)MovieListType.ImdbTop250);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override ClickableWithPagingAdapter<TSearchMovie> GetAdapter()
        {
            return new ImdbMoviesAdapter();
        }
    }

    public class TmdbTopRatedMoviesFragment : MoviesFragment
    {
        public static MoviesFragment NewInstance()
        {
            var frag1 = new TmdbTopRatedMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieListType", (int)MovieListType.TopRated);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override ClickableWithPagingAdapter<TSearchMovie> GetAdapter()
        {
            return new TmdbTopRatedMoviesAdapter();
        }
    }

    public class MoviesFragment : RecyclerViewFragment
    {

        protected ClickableWithPagingAdapter<TMDbLib.Objects.Search.SearchMovie> movieAdapter;
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
            ResetPages();
            movieAdapter = GetAdapter();
            movieAdapter.ItemClick += OnItemClick;
            return movieAdapter;
        }

        protected virtual ClickableWithPagingAdapter<TSearchMovie> GetAdapter() => new MoviesAdapter();

        protected virtual void ResetPages()
        {
            PagedMovie.Instance.Reset(MovieListType);
        }

        protected override void SetupOnScroll()
        {
            var onScrollListener = new RecyclerViewOnScrollListener();
            onScrollListener.LoadMoreEvent += (object sender, EventArgs e) =>
            {
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

        protected override void OnItemClick(object sender, int position)
        {
            try
            {
                base.OnItemClick(sender, position);
                var movie = (sender as MoviesAdapter).movies[position];
                Intent intent = new Intent(this.Context, typeof(MovieInfoActivity));
                Bundle b = new Bundle();
                b.PutInt("movieId", movie.Id);
                b.PutString("movieName", movie.Title);
                b.PutString("movieReleaseDate", movie.ReleaseDate.HasValue ? movie.ReleaseDate.ToString() : null);
                b.PutString("movieLanguage", movie.OriginalLanguage);
                var backdrop = movie.BackdropPath;
                b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
                intent.PutExtras(b);
                StartActivity(intent);
                Activity.OverridePendingTransition(Resource.Animation.@Side_in_right, Resource.Animation.@Side_out_left);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            IMenuItem filter = menu.FindItem(Resource.Id.filter);
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
    }

    public enum MovieListType
    {
        NowPlaying,
        Upcoming,
        //Trending,
        Popular,
        TopRated,
        ImdbTop250
    }
}