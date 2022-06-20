using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieBuddy
{
    public class VideosFragment1 : MoviesFragment
    {
        public static VideosFragment1 NewInstance()
        {
            var frag1 = new VideosFragment1();
            Bundle bundle = new Bundle();
            frag1.Arguments = bundle;
            return frag1;
        }

        int page = 0;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieManager.Instance.GetMovies(Globals.StarredMovies.Skip((page++) * 9).Take(9).ToList());
        }

        protected override void ResetPages() { }
    }

    public class VideosFragment : MoviesFragment
    {
        public static VideosFragment NewInstance(string movieName = null, int movieId = 0, string relaseDate = null, string lang = null)
        {
            var frag1 = new VideosFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("movieName", movieName);
            bundle.PutString("relaseDate", relaseDate);
            bundle.PutString("lang", lang);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            ResetPages();
            movieAdapter = new VideosAdapter();
            movieAdapter.ItemClick += OnItemClick;
            return movieAdapter;
        }

        int page = 1;
        protected override void GetData()
        {
            var date = Arguments.GetString("relaseDate");
            DateTime? relaseDate = null;
            if (!string.IsNullOrWhiteSpace(date))
                relaseDate = DateTime.Parse(date);
            //DateTime ? relaseDate = !string.IsNullOrWhiteSpace(date) ? DateTime.Parse(date) : null;
            var lang = Arguments.GetString("lang");
            var data = MovieManager.Instance.GetVideos(MovieId, MovieName, relaseDate, lang, page++);
            if (data == null) return;
            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            movieAdapter.LoadVideos(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        }


        protected override void OnItemClick(object sender, int position)
        {
            try
            {
                var videoId = (sender as VideosAdapter).videos[position];
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://www.youtube.com/embed/{videoId}"));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        protected override int SpanCount
        {
            get
            {
                return Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait ? 1 : 2;
            }
        }

        protected override void ResetPages() { }
    }
}