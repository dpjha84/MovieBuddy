using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using System;
using Android.Gms.Ads;
using Jaeger;
using static Android.Support.V7.Widget.SearchView;
using Android.Support.V4.App;

namespace MovieBuddy
{
    [Activity(Label = "SearchActivity")]
    public class SearchActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.Toolbar toolbar;
        private SearchPagerAdapter tabPagerAdapter;
        private ViewPager mViewPager;
        private TabLayout mTabLayout;
        protected AdView mAdView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                string query = Intent.GetStringExtra("query");

                savedInstanceState = new Bundle();
                savedInstanceState.PutString("query", query);

                this.Title = "";
                //StatusBarUtil.SetTransparent(this);
                SetContentView(Resource.Layout.Search);

                //mAdView = FindViewById<AdView>(Resource.Id.adView);
                //var adRequest = new AdRequest.Builder().Build();
                //mAdView.LoadAd(adRequest);

                toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

                mViewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
                mViewPager.OffscreenPageLimit = 0;
                mTabLayout = (TabLayout)FindViewById(Resource.Id.tabs);

                tabPagerAdapter = new SearchPagerAdapter(this, SupportFragmentManager, "");

                mViewPager.Adapter = tabPagerAdapter;
                mTabLayout.SetupWithViewPager(mViewPager);

                for (int i = 0; i < mTabLayout.TabCount; i++)
                {
                    TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                    tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
                }

                SetToolbar();

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private void SetToolbar()
        {
            var searchView = FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.searchView);
            searchView.Iconified = false;
            searchView.SetOnQueryTextListener(new QueryTextListener(this, mViewPager, mTabLayout, toolbar, SupportFragmentManager));
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        class QueryTextListener : Java.Lang.Object, IOnQueryTextListener
        {
            ViewPager mViewPager;
            TabLayout mTabLayout;
            Android.Support.V7.Widget.Toolbar toolbar;
            Context context;
            Android.Support.V4.App.FragmentManager mgr;
            public QueryTextListener(Context c, ViewPager pager, TabLayout layout, Android.Support.V7.Widget.Toolbar t, Android.Support.V4.App.FragmentManager m)
            {
                mViewPager = pager;
                mTabLayout = layout;
                toolbar = t;
                context = c;
                mgr = m;
            }
            public bool OnQueryTextChange(string newText)
            {
                var tabPagerAdapter = new SearchPagerAdapter(context, mgr, newText);

                mViewPager.Adapter = tabPagerAdapter;
                mTabLayout.SetupWithViewPager(mViewPager);

                for (int i = 0; i < mTabLayout.TabCount; i++)
                {
                    TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                    tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
                }
                return true;
            }

            public bool OnQueryTextSubmit(string newText)
            {
                return false;
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
    }
}