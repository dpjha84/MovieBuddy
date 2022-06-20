using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Widget;

namespace MovieBuddy
{

    [Activity(Label = "CastInfoActivity")]
    public class CastInfoActivity : ActivityBase
    {
        protected override void OnCreate(Bundle bundle)
        {
            PreCreate();
            ImageToScreenRatio = 0.5F;
            InitView(Resource.Layout.MovieInfoView, bundle);

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            string castName = Intent.GetStringExtra("castName");
            int castId = Intent.GetIntExtra("castId", 0);
            Title = castName;

            var image = FindViewById<ImageView>(Resource.Id.backdrop);
            Helper.SetImage(this, Intent.GetStringExtra("imageUrl"), image, Resource.Drawable.NoCast);
            image.Click += Image_Click; ;

            var mViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            var mTabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            var tabPagerAdapter = new CastPagerAdapter(this, SupportFragmentManager, castName, castId);
            mViewPager.Adapter = tabPagerAdapter;
            mTabLayout.SetupWithViewPager(mViewPager);

            for (int i = 0; i < mTabLayout.TabCount; i++)
            {
                TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
            }
        }

        private void Image_Click(object sender, System.EventArgs e)
        {
            Bundle b = new Bundle();
            Intent intent = new Intent(this, typeof(ImageViewer));
            b.PutString("url", Intent.GetStringExtra("imageUrl"));
            intent.PutExtras(b);
            StartActivity(intent);
        }
    }
}