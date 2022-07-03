using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
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
            //ResetPages();
            movieAdapter = new VideosAdapter();
            movieAdapter.ItemClick += OnItemClick;
            movieAdapter.YoutubeClick += OnPosterClick;
            return movieAdapter;
        }

        private int page = 1;
        protected override void GetData()
        {
            var date = Arguments.GetString("relaseDate");
            DateTime? relaseDate = null;
            if (!string.IsNullOrWhiteSpace(date))
                relaseDate = DateTime.Parse(date);
            var lang = Arguments.GetString("lang");
            if (MovieId > 0 && page > 1)
                return;
            var data = MovieManager.Instance.GetVideos(MovieId, MovieName, relaseDate, lang, page++);
            if (data == null)
                return;

            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            movieAdapter.LoadVideos(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        }

        protected override void OnPosterClick(object sender, int position)
        {
            var videoId = (sender as VideosAdapter).videos[position].VideoId;
            var movie = (sender as VideosAdapter).videos[position].Movie;
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

        protected override void OnItemClick(object sender, int position)
        {
            try
            {
                //DoAfterAd(() =>
                //{
                    var vh = rv.FindViewHolderForAdapterPosition(position) as VideosViewHolder;
                    //vh.WebView.Visibility = ViewStates.Visible;
                    var videoId = (sender as VideosAdapter).videos[position].VideoId;

                    Intent intent = new Intent(this.Context, typeof(VideoViewer));
                    Bundle b = new Bundle();
                    b.PutString("videoId", videoId);
                    intent.PutExtras(b);
                    StartActivity(intent);
                    //Activity.OverridePendingTransition(Resource.Animation.@Side_in_right, Resource.Animation.@Side_out_left);

                    //var vh = rv.FindViewHolderForAdapterPosition(position) as VideosViewHolder;
                    //vh.WebView.Visibility = ViewStates.Visible;
                    //var videoId = (sender as VideosAdapter).videos[position];
                    //WebSettings webSettings = vh.WebView.Settings;
                    //webSettings.JavaScriptEnabled = true;
                    //webSettings.MediaPlaybackRequiresUserGesture = false;
                    //webSettings.CacheMode = CacheModes.CacheElseNetwork;
                    ////vh.WebView.SetWebViewClient(new MyWebViewClient());
                    //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
                    //    vh.WebView.SetLayerType(LayerType.Hardware, null);
                    //else
                    //    vh.WebView.SetLayerType(LayerType.Software, null);

                    ////vh.WebView.SetWebChromeClient(new FullScreenClient(vh.ParentLayout, vh.ContentLayout));
                    ////webView.SetWebChromeClient(new WebChromeClient());
                    //webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
                    //vh.WebView.SetWebViewClient(new WebViewClient());
                    //vh.WebView.SetWebChromeClient(new WebChromeClient());
                    //webSettings.SavePassword = true;
                    //webSettings.SaveFormData = true;
                    //webSettings.SetEnableSmoothTransition(true);
                    //webSettings.LoadWithOverviewMode = true;
                    //webSettings.UseWideViewPort = true;
                    //webSettings.SetRenderPriority(WebSettings.RenderPriority.High);
                    //webSettings.SetAppCacheEnabled(true);
                    //vh.WebView.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
                    //webSettings.DomStorageEnabled = true;

                    //vh.WebView.LoadUrl($"file:///android_asset/player.html?videoId={videoId}");
                //});
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private void DoAfterAd(Action action)
        {
            var FinalAd = AdWrapper.ConstructFullPageAdd(this.Context, "ca-app-pub-3940256099942544/1033173712");
            var intlistener = new adlistener();
            intlistener.AdLoaded += () =>
            {
                if (FinalAd.IsLoaded)
                {
                    FinalAd.Show();
                }
            };
            intlistener.AdClosed += () =>
            {
                action();
            };
            FinalAd.AdListener = intlistener;
            FinalAd.CustomBuild();
        }

        protected override int SpanCount
        {
            get
            {
                return Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait ? 1 : 3;
            }
        }

        protected override void ResetPages() { }
    }
}