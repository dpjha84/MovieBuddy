using Android.App;
using Android.Gms.Ads;
using Android.OS;

namespace MovieBuddy
{
    [Activity(Label = "Popular People")]
    public class PopularPersonActivity : StaticThemeActivity
    {
        public PopularPersonActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Popular People";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, PopularPersonFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }
}