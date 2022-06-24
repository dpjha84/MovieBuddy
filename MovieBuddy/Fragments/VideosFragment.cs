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
            movieAdapter.YoutubeClick += OnYoutubeItemClick;
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

        protected override void OnYoutubeItemClick(object sender, int position)
        {
            var videoId = (sender as VideosAdapter).videos[position];
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://www.youtube.com/embed/{videoId}"));
            StartActivity(intent);
        }

        protected override void OnItemClick(object sender, int position)
        {
            try
            {
                var vh = rv.FindViewHolderForAdapterPosition(position) as VideosViewHolder;
                vh.WebView.Visibility = ViewStates.Visible;
                var videoId = (sender as VideosAdapter).videos[position];
                WebSettings webSettings = vh.WebView.Settings;
                webSettings.JavaScriptEnabled = true;
                webSettings.MediaPlaybackRequiresUserGesture = false;
                webSettings.CacheMode = CacheModes.CacheElseNetwork;
                //vh.WebView.SetWebViewClient(new MyWebViewClient());
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
                    vh.WebView.SetLayerType(LayerType.Hardware, null);
                else
                    vh.WebView.SetLayerType(LayerType.Software, null);

                //vh.WebView.SetWebChromeClient(new FullScreenClient(vh.ParentLayout, vh.ContentLayout));
                //webView.SetWebChromeClient(new WebChromeClient());
                webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
                vh.WebView.SetWebViewClient(new WebViewClient());
                vh.WebView.SetWebChromeClient(new WebChromeClient());
                webSettings.SavePassword = true;
                webSettings.SaveFormData = true;
                webSettings.SetEnableSmoothTransition(true);
                webSettings.LoadWithOverviewMode = true;
                webSettings.UseWideViewPort = true;
                webSettings.SetRenderPriority(WebSettings.RenderPriority.High);
                webSettings.SetAppCacheEnabled(true);
                vh.WebView.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
                webSettings.DomStorageEnabled = true;

                vh.WebView.LoadUrl($"file:///android_asset/player.html?videoId={videoId}");
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public class MyWebViewClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                view.LoadUrl("javascript:(function() { document.getElementsByTagName('video')[0].play(); })()");
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