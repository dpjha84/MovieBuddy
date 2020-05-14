using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System.Linq;
using System.Text.RegularExpressions;

namespace MovieBuddy
{
    public class TrailerFragment : BaseFragment
    {
        int intDisplayWidth;
        int intDisplayHeight;
        public static TrailerFragment NewInstance(string movieName, int movieId)
        {
            var frag1 = new TrailerFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("movieName", movieName);
            frag1.Arguments = bundle;
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("movieName");
            movieId = Arguments.GetInt("movieId");

            View rootView = inflater.Inflate(Resource.Layout.layout1, container, false);
            var trailerId = MovieManager.Instance.GetTrailer(movieId, movieName);
            if (!string.IsNullOrWhiteSpace(trailerId))
            {
                var rv = (WebView)rootView.FindViewById(Resource.Id.webView1);
                rv.Settings.JavaScriptEnabled = true;
                rv.SetWebChromeClient(new WebChromeClient());
                rv.LoadUrl($"https://www.youtube.com/embed/{trailerId}");
            }
            return rootView;
        }

        void FnPlayInWebView(View rootView)
        {
            string strUrl = "https://www.youtube.com/watch?v=rOUAWzxVwX0";

            string id = FnGetVideoID(strUrl);

            if (!string.IsNullOrEmpty(id))
            {
                strUrl = string.Format("http://youtube.com/embed/{0}", id);
            }
            else
            {
                Toast.MakeText(this.Context, "Video url is not in correct format", ToastLength.Long).Show();
                return;
            }

            string html = @"<html><body><iframe  src=""strUrl"" allowfullscreen=""allowfullscreen"" mozallowfullscreen=""mozallowfullscreen"" msallowfullscreen=""msallowfullscreen"" oallowfullscreen=""oallowfullscreen"" webkitallowfullscreen=""webkitallowfullscreen""></iframe></body></html>";
            var myWebView = (WebView)rootView.FindViewById(Resource.Id.webView1);
            var settings = myWebView.Settings;
            settings.JavaScriptEnabled = true;
            settings.UseWideViewPort = true;
            settings.LoadWithOverviewMode = true;
            settings.JavaScriptCanOpenWindowsAutomatically = true;
            settings.DomStorageEnabled = true;
            settings.SetRenderPriority(WebSettings.RenderPriority.High);
            settings.BuiltInZoomControls = false;

            settings.JavaScriptCanOpenWindowsAutomatically = true;
            myWebView.SetWebChromeClient(new WebChromeClient());
            settings.AllowFileAccess = true;
            settings.SetPluginState(WebSettings.PluginState.On);
            string strYouTubeURL = html.Replace("videoWidth", intDisplayWidth.ToString()).Replace("videoHeight", intDisplayHeight.ToString()).Replace("strUrl", strUrl);

            myWebView.LoadDataWithBaseURL(null, strYouTubeURL, "text/html", "UTF-8", null);

        }

        int FnConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        static string FnGetVideoID(string strVideoURL)
        {
            const string regExpPattern = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";
            //for Vimeo: vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)
            var regEx = new Regex(regExpPattern);
            var match = regEx.Match(strVideoURL);
            return match.Success ? match.Groups[1].Value : null;
        }
    }    
}