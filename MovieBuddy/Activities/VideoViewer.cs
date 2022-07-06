using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Xamarin.Essentials;

namespace MovieBuddy
{
    [Activity(Label = "VideoViewer")]
    public class VideoViewer : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VideoViewer);

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            var webView = FindViewById<WebView>(Resource.Id.mWebView);

            //string myYouTubeVideoUrl = $"https://www.youtube.com/embed/{Intent.GetStringExtra("videoId")}?autoplay=1";

            //string dataUrl =
            //        "<html>" +
            //                "<iframe width=\"100%\" height=\"100%\" src=\"" + myYouTubeVideoUrl + "\" frameborder=\"0\" allowfullscreen/>" +
            //        "</html>";

            //WebSettings webSettings = webView.Settings;

            //webSettings.JavaScriptEnabled = true;
            //webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            //webSettings.LoadWithOverviewMode = true;
            //webSettings.UseWideViewPort = true;
            //webView.LoadData(dataUrl, "text/html", "utf-8");

            WebSettings webSettings = webView.Settings;
            webSettings.JavaScriptEnabled = true;
            webSettings.MediaPlaybackRequiresUserGesture = false;
            webSettings.CacheMode = CacheModes.CacheElseNetwork;
            //vh.WebView.SetWebViewClient(new MyWebViewClient());
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                webView.SetLayerType(LayerType.Hardware, null);
            else
                webView.SetLayerType(LayerType.Software, null);

            //vh.WebView.SetWebChromeClient(new FullScreenClient(vh.ParentLayout, vh.ContentLayout));
            //webView.SetWebChromeClient(new WebChromeClient());
            webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            webView.SetWebViewClient(new WebViewClient());
            webView.SetWebChromeClient(new WebChromeClient());
            webSettings.SavePassword = true;
            webSettings.SaveFormData = true;
            webSettings.SetEnableSmoothTransition(true);
            webSettings.LoadWithOverviewMode = true;
            webSettings.UseWideViewPort = true;
            webSettings.SetRenderPriority(WebSettings.RenderPriority.High);
            webSettings.SetAppCacheEnabled(true);
            webView.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            webSettings.DomStorageEnabled = true;
            //webView.SetInitialScale(GetScale());
            //if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
            webView.LoadUrl($"file:///android_asset/player.html?videoId={Intent.GetStringExtra("videoId")}");
            //else
            //    webView.LoadUrl($"file:///android_asset/player1.html?videoId={Intent.GetStringExtra("videoId")}");
        }

        private int GetScale()
        {
            double width = DeviceDisplay.MainDisplayInfo.Width;
            double val = width / DeviceDisplay.MainDisplayInfo.Height;
            val = val * 100d;
            return (int)val;
        }
    }
}