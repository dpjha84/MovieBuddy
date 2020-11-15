using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;

namespace MovieBuddy
{
    [Activity(Label = "Favourite Movies")]
    public class StarredMoviesActivity : ActivityBase
    {
        public StarredMoviesActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Favourite Movies";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, StarredMoviesFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }

    [Activity(Label = "Already Watched Movies")]
    public class AlreadyWatchedMoviesActivity : ActivityBase
    {
        public AlreadyWatchedMoviesActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Already Watched Movies";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.frameLayout1, AlreadyWatchedMoviesFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }

    [Activity(Label = "To Watch Movies")]
    public class ToWatchMoviesActivity : ActivityBase
    {
        public ToWatchMoviesActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "To Watch Movies";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, ToWatchMoviesFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }
}