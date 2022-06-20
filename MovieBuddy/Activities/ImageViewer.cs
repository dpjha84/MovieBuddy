
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Widget;

namespace MovieBuddy
{
    [Activity(Label = "FullScreenViewer")]
    public class ImageViewer : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ImageViewer);
            var image = FindViewById<ImageView>(Resource.Id.imgViewer);
            Helper.SetImage(this, Intent.GetStringExtra("url"), image, Resource.Drawable.noimage);
            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);
        }
    }
}