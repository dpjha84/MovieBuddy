using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace MovieBuddy
{
    public class MoviesFragment : RecyclerViewFragment
    {
        protected MovieListType MovieListType { get { return (MovieListType)Arguments.GetInt("movieListType"); } }

        public static MoviesFragment NewInstance(MovieListType type)
        {
            var frag1 = new MoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieListType", (int)type);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override void SetAdapter()
        {
            adapter = new MoviesAdapter(GetMovies());
            adapter.ItemClick += OnItemClick;
        }

        protected virtual List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieListType switch
            {
                MovieListType.NowPlaying => MovieManager.Instance.NowPlaying,
                MovieListType.Upcoming => MovieManager.Instance.Upcoming,
                //MovieListType.Trending => MovieManager.Instance.Trending,
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
        //Trending,
        Popular,
        TopRated
    }
}