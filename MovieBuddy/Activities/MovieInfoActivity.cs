using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Jaeger;
using System;

namespace MovieBuddy
{
    [Activity(Label = "MovieInfoActivity")]
    public class MovieInfoActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.Toolbar toolbar;
        private ImageView imageView;
        private CollapsingToolbarLayout collapsingToolbar;
        private MoviePagerAdapter tabPagerAdapter;
        private ViewPager mViewPager;
        private TabLayout mTabLayout;
        protected AdView mAdView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                string movieName = Intent.GetStringExtra("movieName");
                int movieId = Intent.GetIntExtra("movieId", 0);
                string castName = Intent.GetStringExtra("castName");
                int castId = Intent.GetIntExtra("castId", 0);
                bool isCast = Intent.GetBooleanExtra("isCast", false);

                savedInstanceState = new Bundle();
                savedInstanceState.PutInt("movieId", movieId);
                savedInstanceState.PutString("name", movieName);
                savedInstanceState.PutInt("castId", castId);
                savedInstanceState.PutBoolean("isCast", isCast);

                this.Title = movieName;
                StatusBarUtil.SetTransparent(this);
                SetContentView(Resource.Layout.MovieInfoView);

                //mAdView = FindViewById<AdView>(Resource.Id.adView);
                //var adRequest = new AdRequest.Builder().Build();
                //mAdView.LoadAd(adRequest);


                imageView = (ImageView)FindViewById(Resource.Id.backdrop);

                collapsingToolbar = (CollapsingToolbarLayout)FindViewById(Resource.Id.collapsing_toolbar);

                SetToolbar();
                SetImage();

                mViewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
                mViewPager.OffscreenPageLimit = 0;
                mTabLayout = (TabLayout)FindViewById(Resource.Id.tabs);
                tabPagerAdapter = new MoviePagerAdapter(this, SupportFragmentManager, movieName, movieId, Intent.GetStringExtra("imageUrl"));

                mViewPager.Adapter = tabPagerAdapter;
                mTabLayout.SetupWithViewPager(mViewPager);

                for (int i = 0; i < mTabLayout.TabCount; i++)
                {
                    TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                    tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void SetToolbar()
        {
            toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }
        }

        private void SetImage()
        {
            Helper.SetImage(this, Intent.GetStringExtra("imageUrl"), imageView, Resource.Drawable.noimage);
        }

        protected override void OnStop()
        {
            Helper.Clear(this, imageView);
            //for (int i = 0; i < mTabLayout.TabCount; i++)
            //{
            //    Helper.Clear(this, mTabLayout.GetTabAt(i).CustomView);
            //}
            base.OnStop();
        }

        protected override void OnRestart()
        {
            SetImage();
            base.OnRestart();
        }
    }
}