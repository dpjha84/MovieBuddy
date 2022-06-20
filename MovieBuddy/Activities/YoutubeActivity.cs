using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace MovieBuddy.Activities
{
    [Activity(Label = "YoutubeActivity")]
    public class YoutubeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.trailer1);
            var webView = FindViewById<WebView>(Resource.Id.webView1);
            var linearLayout = FindViewById<LinearLayout>(Resource.Id.ll1);
            var contentLayout = FindViewById<LinearLayout>(Resource.Id.ll2);

            var url = "<iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/rX_Xr-F-hEQ\" frameborder=\"0\" allowfullscreen/>";

            WebSettings webSettings = webView.Settings;

            webSettings.JavaScriptEnabled = true;
            webView.SetWebChromeClient(new FullScreenClient(linearLayout, contentLayout));
            //webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.Normal);
            webSettings.LoadWithOverviewMode = true;
            webSettings.UseWideViewPort = true;

            webView.LoadData(url, "text/html", "utf-8");
            //Helper.SetImage(this, Intent.GetStringExtra("url"), webView, Resource.Drawable.noimage);
            // Create your application here
        }
    }

    public class FullScreenClient : WebChromeClient
    {
        private readonly FrameLayout.LayoutParams matchParentLayout = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                                                                         ViewGroup.LayoutParams.MatchParent);
        private readonly LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                                           ViewGroup.LayoutParams.MatchParent);
        private readonly ViewGroup content;
        private readonly ViewGroup parent;
        private View customView;

        public FullScreenClient(ViewGroup parent, ViewGroup content)
        {
            this.parent = parent;
            this.content = content;
        }

        public override void OnShowCustomView(View view, ICustomViewCallback callback)
        {
            customView = view;
            view.LayoutParameters = matchParentLayout;
            //view.LayoutParameters = layoutParams;
            parent.AddView(view);
            content.Visibility = ViewStates.Gone;
        }

        public override void OnHideCustomView()
        {
            content.Visibility = ViewStates.Visible;
            parent.RemoveView(customView);
            customView = null;
        }
    }
}