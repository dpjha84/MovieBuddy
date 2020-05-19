using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Gms.Ads;

namespace MovieBuddy
{
    [Activity(Label = "CastInfoActivity")]
    public class CastInfoActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.Toolbar toolbar;
        private ImageView imageView;
        private CollapsingToolbarLayout collapsingToolbar;
        private CastPagerAdapter tabPagerAdapter;
        private ViewPager mViewPager;
        private TabLayout mTabLayout;
        protected AdView mAdView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);

            string castName = Intent.GetStringExtra("castName");
            int castId = Intent.GetIntExtra("castId", 0);

            this.Title = castName;
            SetContentView(Resource.Layout.MovieInfoView);

            //mAdView = FindViewById<AdView>(Resource.Id.adView);
            //var adRequest = new AdRequest.Builder().Build();
            //mAdView.LoadAd(adRequest);

            
             
            imageView = (ImageView)FindViewById(Resource.Id.backdrop);

            collapsingToolbar = (CollapsingToolbarLayout)FindViewById(Resource.Id.collapsing_toolbar);
            //collapsingToolbar.SetTitle(castName);

            SetToolbar();
            SetImage();

            mViewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
            mTabLayout = (TabLayout)FindViewById(Resource.Id.tabs);
            tabPagerAdapter = new CastPagerAdapter(this, SupportFragmentManager, castName, castId);
            mViewPager.Adapter = tabPagerAdapter;
            mTabLayout.SetupWithViewPager(mViewPager);

            for (int i = 0; i < mTabLayout.TabCount; i++)
            {
                TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
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
            string backdrop = Intent.GetStringExtra("imageUrl");
            Helper.SetImage(this, backdrop, imageView, Resource.Drawable.NoCast);
        }
    }
}