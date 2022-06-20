using Android.App;
using Android.Gms.Ads;
using Android.OS;

namespace MovieBuddy
{
    [Activity(Label = "Favourites")]
    public class StarredMoviesActivity : StaticThemeActivity
    {
        public StarredMoviesActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Favourites";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, StarredMoviesFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }

    [Activity(Label = "Watch History")]
    public class AlreadyWatchedMoviesActivity : StaticThemeActivity
    {
        public AlreadyWatchedMoviesActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Watch History";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.frameLayout1, AlreadyWatchedMoviesFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }

    [Activity(Label = "Watch List")]
    public class ToWatchMoviesActivity : StaticThemeActivity
    {
        public ToWatchMoviesActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.PopularPeople, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Watch List";
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, ToWatchMoviesFragment.NewInstance());
            fragmentTransaction.Commit();
        }
    }
}